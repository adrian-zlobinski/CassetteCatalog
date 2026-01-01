using CassetteCatalog.Core.Models;
using System.ComponentModel;

namespace CassetteCatalog.Wpf.ViewModels
{
    public class AlbumNode : INotifyPropertyChanged
    {
        public Album Album { get; set; }
        public string Title => Album.Title;

        public AlbumNode(Album album)
        {
            Album = album;
        }

        public void Refresh() => OnPropertyChanged(nameof(Title));

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
