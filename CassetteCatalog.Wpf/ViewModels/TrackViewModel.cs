using CassetteCatalog.Core.Enums;
using CassetteCatalog.Core.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CassetteCatalog.Wpf.ViewModels
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        public Track TrackModel { get; }

        public TrackViewModel(Track track)
        {
            this.TrackModel = track;
        }

        public int Number
        {
            get => TrackModel.Number;
            set
            {
                if (TrackModel.Number != value)
                {
                    TrackModel.Number = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Title {
            get => TrackModel.Title;
            set
            {
                if (TrackModel.Title != value)
                {
                    TrackModel.Title = value;
                    OnPropertyChanged();
                }
            }
        }
        public eCassetteSide Side
        {
            get => TrackModel.Side;
            set
            {
                if (TrackModel.Side != value)
                {
                    TrackModel.Side = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan Duration
        {
            get => TrackModel.Duration;
            set
            {
                if (TrackModel.Duration != value)
                {
                    TrackModel.Duration = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
