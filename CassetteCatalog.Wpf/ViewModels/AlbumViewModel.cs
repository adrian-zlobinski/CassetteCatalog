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

namespace CassetteCatalog.Wpf.ViewModels
{
    public class AlbumViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;

        public ObservableCollection<Album> Albums { get; set; } = new ObservableCollection<Album>();

        private Album? _selectedAlbum;
        public Album? SelectedAlbum
        {
            get => _selectedAlbum;
            set
            {
                if (_selectedAlbum != value)
                {
                    _selectedAlbum = value;
                    OnPropertyChanged();
                }
            }
        }

        public AlbumViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadAlbums();
        }

        public void LoadAlbums()
        {
            Albums.Clear();
            var albums = _dbContext.Albums.ToList();
            foreach (var album in albums)
            {
                Albums.Add(album);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
