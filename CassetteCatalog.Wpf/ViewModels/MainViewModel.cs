using CassetteCatalog.Core.Enums;
using CassetteCatalog.Core.Models;
using CassetteCatalog.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CassetteCatalog.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly AppDbContext _dbContext;
        public ICommand AddAlbumCommand { get;  }
        public ICommand EditAlbumCommand { get; }
        public ICommand DeleteAlbumCommand { get; }

        public ObservableCollection<ArtistNode> Artists { get; } = new();

        private Album? _selectedAlbum;
        public Album SelectedAlbum
        {
            get => _selectedAlbum!;
            set
            {
                if (_selectedAlbum != value)
                {
                    _selectedAlbum = value;
                    OnPropertyChanged();
                    ((RelayCommand)EditAlbumCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteAlbumCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string AppTitle { get; }
        public string FooterInfo { get; }

        public MainViewModel(AppDbContext dbContext)
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            var author = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            AppTitle = $"Cassette Albums Catalog v{version}";
            FooterInfo = $"© 2026 {author}";
            _dbContext = dbContext;
            AddAlbumCommand = new RelayCommand(AddAlbum);
            EditAlbumCommand = new RelayCommand(EditAlbum, () => SelectedAlbum != null);
            DeleteAlbumCommand = new RelayCommand(DeleteAlbum, () => SelectedAlbum != null);

            LoadTree();
        }

        private void LoadTree()
        {
            var albums = _dbContext.Albums.AsNoTracking().ToList();
            var groups = albums.GroupBy(a => a.Artist).OrderBy(g => g.Key);

            Artists.Clear();

            foreach (var group in groups)
            {
                // Przekazujemy nazwę artysty i listę albumów do konstruktora
                var artistNode = new ArtistNode(group.Key, group.ToList());
                Artists.Add(artistNode);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void AddAlbum()
        {
            var editVm = new AlbumEditViewModel();
            var window = new AlbumEditWindow { DataContext = editVm, Owner = App.Current.MainWindow };

            if (window.ShowDialog() == true)
            {
                var newAlbum = new Album
                {
                    Artist = editVm.Artist,
                    Title = editVm.Title,
                    ReleaseYear = editVm.ReleaseYear,
                    TapeType = editVm.SelectedTapeType,
                    CassetteName = editVm.CassetteName,
                    Tracks = editVm.Tracks.Select(tvm => tvm.TrackModel).ToList()
                };

                // 2. Zapis do bazy danych (tu album otrzymuje swoje unikalne Id)
                _dbContext.Albums.Add(newAlbum);
                _dbContext.SaveChanges();
                // 3. Aktualizacja Widoku (UI)
                // Szukamy, czy w drzewie istnieje już Node dla tego artysty
                var artistNode = Artists.FirstOrDefault(a =>
                    a.Name.Equals(newAlbum.Artist, StringComparison.OrdinalIgnoreCase));

                if (artistNode != null)
                {
                    // Artysta istnieje - dodajemy album do jego kolekcji
                    var albumNode = new AlbumNode(newAlbum);
                    artistNode.Albums.Add(albumNode);
                }
                else
                {
                    // Nowy artysta - musimy stworzyć nową gałąź w drzewie
                    var newArtistNode = new ArtistNode(newAlbum.Artist, new List<Album> { newAlbum });

                    // Wstawiamy alfabetycznie, żeby zachować porządek
                    var index = Artists.TakeWhile(a => string.Compare(a.Name, newAlbum.Artist) < 0).Count();
                    Artists.Insert(index, newArtistNode);
                }

                // 4. Automatyczne zaznaczenie nowego elementu
                SelectedAlbum = newAlbum;
            }
        }
        private void EditAlbum()
        {
            if (SelectedAlbum == null) return;

            // Pobieramy świeżą wersję z bazy (bez AsNoTracking)
            var dbAlbum = _dbContext.Albums
                .Include(a => a.Tracks)
                .FirstOrDefault(a => a.Id == SelectedAlbum.Id);

            if (dbAlbum == null) return;

            var editVm = new AlbumEditViewModel();
            editVm.Artist = dbAlbum.Artist;
            editVm.Title = dbAlbum.Title;
            editVm.ReleaseYear = dbAlbum.ReleaseYear;
            editVm.CassetteName = dbAlbum.CassetteName;
            editVm.SelectedTapeType = dbAlbum.TapeType;
            foreach(var track in dbAlbum.Tracks.OrderBy(t => t.Number))
            {
                editVm.Tracks.Add(new TrackViewModel(track));
            }

            var window = new AlbumEditWindow { DataContext = editVm, Owner = App.Current.MainWindow };

            if (window.ShowDialog() == true)
            {
                string oldArtist = dbAlbum.Artist;

                dbAlbum.Artist = editVm.Artist;
                dbAlbum.Title = editVm.Title;
                dbAlbum.ReleaseYear = editVm.ReleaseYear;
                dbAlbum.CassetteName = editVm.CassetteName;
                dbAlbum.TapeType = editVm.SelectedTapeType;
                // Aktualizacja listy utworów
                dbAlbum.Tracks.Clear();
                foreach (var trackVm in editVm.Tracks)
                {
                    dbAlbum.Tracks.Add(trackVm.TrackModel);
                }

                _dbContext.SaveChanges();

                if(oldArtist.Equals(dbAlbum.Artist, StringComparison.OrdinalIgnoreCase))
                {
                    var artistNode = Artists.FirstOrDefault(a => a.Name.Equals(dbAlbum.Artist, StringComparison.OrdinalIgnoreCase));
                    var albumNode = artistNode?.Albums.FirstOrDefault(a => a.Album.Id == dbAlbum.Id);
                    albumNode?.Refresh(dbAlbum);
                }
                else
                {
                    MoveAlbumInTree(dbAlbum, oldArtist);
                }
            }
        }

        private void MoveAlbumInTree(Album album, string oldArtistName)
        {
            var oldArtistNode = Artists.FirstOrDefault(a => a.Name.Equals(oldArtistName, StringComparison.OrdinalIgnoreCase));
            if (oldArtistName != null)
            {
                var albumNode = oldArtistNode.Albums.FirstOrDefault(a => a.Album.Id == album.Id);
                if (albumNode != null)
                {
                    oldArtistNode.Albums.Remove(albumNode);
                }
                if (!oldArtistNode.Albums.Any())
                {
                    Artists.Remove(oldArtistNode);
                }
            }
            var newArtistNode = Artists.FirstOrDefault(a => a.Name.Equals(album.Artist, StringComparison.OrdinalIgnoreCase));
            if(newArtistNode != null)
            {
                newArtistNode.Albums.Add(new AlbumNode(album));
            }
            else
            {
                var newArtist = new ArtistNode(album.Artist, new List<Album> { album });
                var index = Artists.TakeWhile(a => string.Compare(a.Name, album.Artist, StringComparison.OrdinalIgnoreCase) < 0).Count();
                Artists.Insert(index, newArtist);
            }
        }
        private void DeleteAlbum()
        {
            if (SelectedAlbum == null) return;

            // Znajdujemy album w bazie po unikalnym ID
            var albumToRemove = _dbContext.Albums.FirstOrDefault(a => a.Id == SelectedAlbum.Id);

            if (albumToRemove != null)
            {
                _dbContext.Albums.Remove(albumToRemove);
                _dbContext.SaveChanges();

                // Zamiast LoadTree(), możesz usunąć tylko ten jeden Node dla wydajności
                RemoveAlbumFromView(SelectedAlbum.Id);
                SelectedAlbum = null;
            }
        }

        private void RemoveAlbumFromView(int albumId)
        {
            // 1. Znajdź artystę, u którego jest ten album
            var artistNode = Artists.FirstOrDefault(a => a.Albums.Any(alb => alb.Album.Id == albumId));

            if (artistNode != null)
            {
                // 2. Znajdź i usuń album
                var albumNode = artistNode.Albums.FirstOrDefault(a => a.Album.Id == albumId);
                if (albumNode != null)
                {
                    artistNode.Albums.Remove(albumNode);
                }

                // 3. Jeśli artysta jest pusty, usuń go z głównej listy
                if (artistNode.Albums.Count == 0)
                {
                    Artists.Remove(artistNode);
                }
            }
        }
    }
}
