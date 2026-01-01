using CassetteCatalog.Core.Models;
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
    public class ArtistNode : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public ObservableCollection<AlbumNode> Albums { get; set; }

        public ArtistNode(string name, IEnumerable<Album> albums)
        {
            Name = name;
            Albums = new ObservableCollection<AlbumNode>(
                albums.Select(a=> new AlbumNode(a)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
