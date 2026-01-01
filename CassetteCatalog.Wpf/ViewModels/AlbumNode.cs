using CassetteCatalog.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
