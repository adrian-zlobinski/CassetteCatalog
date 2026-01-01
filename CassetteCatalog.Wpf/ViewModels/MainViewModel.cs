using CassetteCatalog.Core.Enums;
using CassetteCatalog.Core.Models;
using CassetteCatalog.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public MainViewModel(AppDbContext dbContext)
        {
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
                    Tracks = new List<Track>()
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
            if (dbAlbum != null)
            {
                // Tutaj logika aktualizacji pól, np.:
                // dbAlbum.Title = "Nowy Tytuł";
                dbAlbum.Title = "Zmieniony tytuł";
                if(dbAlbum.Tracks == null)
                {
                    dbAlbum.Tracks = new List<Track>();
                }
                dbAlbum.Tracks.Add(new Track()
                {
                    Id = 0,
                    Duration = new TimeSpan(0, 3, 3),
                    Title = "Ścieżka",
                    Side = eCassetteSide.A,
                    Number = 1

                });

                _dbContext.SaveChanges();

                // Powiadomienie UI o zmianie (zakładając Refresh() w AlbumNode)
                var node = Artists.SelectMany(a => a.Albums)
                                          .FirstOrDefault(n => n.Album.Id == SelectedAlbum.Id); node?.Refresh();
                // Ważne: Jeśli masz widok szczegółowy po prawej stronie, 
                // powinieneś też powiadomić o zmianie we właściwości SelectedAlbum
                OnPropertyChanged(nameof(SelectedAlbum));
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
            foreach (var artist in Artists)
            {
                var albumNode = artist.Albums.FirstOrDefault(a => a.Album.Id == albumId);
                if (albumNode != null)
                {
                    artist.Albums.Remove(albumNode);
                    // Jeśli artysta nie ma już albumów, możesz go też usunąć z drzewa
                    if (artist.Albums.Count == 0) Artists.Remove(artist);
                    break;
                }
            }
        }
    }
}
