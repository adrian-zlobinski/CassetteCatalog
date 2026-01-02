using CassetteCatalog.Core.Models;
using System.ComponentModel;

namespace CassetteCatalog.Wpf.ViewModels
{
    public class AlbumNode : INotifyPropertyChanged
    {
        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Title => Album.Title;

        public AlbumNode(Album album)
        {
            _album = album;
        }

        public void Refresh(Album updatedAlbum)
        {
            Album = updatedAlbum;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
