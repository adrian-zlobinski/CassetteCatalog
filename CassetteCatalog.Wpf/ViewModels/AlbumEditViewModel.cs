using CassetteCatalog.Core.Enums;
using CassetteCatalog.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CassetteCatalog.Wpf.ViewModels
{
    public class AlbumEditViewModel : INotifyPropertyChanged
    {
        #region Local Variables
        private TrackViewModel? _selectedTrack;
        private TimeSpan SideADuration => TimeSpan.FromTicks(Tracks.Where(t => t.Side == eCassetteSide.A).Sum(t => t.Duration.Ticks));
        private TimeSpan SideBDuration => TimeSpan.FromTicks(Tracks.Where(t => t.Side == eCassetteSide.B).Sum(t => t.Duration.Ticks));
        private TimeSpan TotalDuration => TimeSpan.FromTicks(Tracks.Sum(t => t.Duration.Ticks));
        #endregion

        #region GUI Variables
        public string Artist { get; set; } = "Nowy Artysta";
        public string Title { get; set; } = "Nowy Tytuł";
        public ushort ReleaseYear { get; set; } = (ushort)DateTime.Now.Year;
        public string CassetteName { get; set; } = "No name";
        public eTapeType SelectedTapeType { get; set; } = eTapeType.TypeI_Fe;
        public IEnumerable<eTapeType> TapeTypes => Enum.GetValues(typeof(eTapeType)).Cast<eTapeType>();

        public string SideADurationFormatted => FormatTimeSpan(SideADuration);
        public string SideBDurationFormatted => FormatTimeSpan(SideBDuration);
        public string TotalDurationFormatted => FormatTimeSpan(TotalDuration);
        public int TrackCount => Tracks.Count;

        public ObservableCollection<TrackViewModel> Tracks { get; set; } = new();
        public TrackViewModel? SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();
                ((RelayCommand)MoveTrackUpCommand).RaiseCanExecuteChanged();
                ((RelayCommand)MoveTrackDownCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteTrackCommand).RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddTrackCommand { get; }
        public ICommand DeleteTrackCommand { get; }
        public ICommand DeleteAllTracksCommand { get; }
        public ICommand MoveTrackUpCommand { get; }
        public ICommand MoveTrackDownCommand { get; }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<bool>? RequestClose;
        #endregion

        public AlbumEditViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AddTrackCommand = new RelayCommand(AddTrack);
            DeleteTrackCommand = new RelayCommand(DeleteTrack, () => SelectedTrack != null);
            DeleteAllTracksCommand = new RelayCommand(() => Tracks.Clear(), () => Tracks.Any());
            MoveTrackUpCommand = new RelayCommand(MoveTrackUp, CanMoveTrackUp);
            MoveTrackDownCommand = new RelayCommand(MoveTrackDown, CanMoveTrackDown);

            foreach(var track in Tracks)
            {
                track.PropertyChanged += Track_PropertyChanged;
            }

            Tracks.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (TrackViewModel t in e.NewItems)
                    {
                        t.PropertyChanged += Track_PropertyChanged;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (TrackViewModel t in e.OldItems)
                    {
                        t.PropertyChanged -= Track_PropertyChanged;
                    }
                }
                RefreshTotals();
            };
            RefreshTotals();
        }
        private void Track_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TrackViewModel.Duration) ||
                e.PropertyName == nameof(TrackViewModel.Side))
            {
                RefreshTotals();                
            }
        }

        private bool CanMoveTrackUp()
        {
            if (SelectedTrack == null) return false;
            return Tracks.IndexOf(SelectedTrack) > 0;
        }
        private bool CanMoveTrackDown()
        {
            if (SelectedTrack == null) return false;
            return Tracks.IndexOf(SelectedTrack) < Tracks.Count - 1;
        }
        private void AddTrack()
        {
            int nextNumber = Tracks.Any() ? Tracks.Max(t => t.Number) + 1 : 1;
            var lastSide = Tracks.LastOrDefault()?.Side ?? eCassetteSide.A;
            Tracks.Add(new TrackViewModel(new Track()
            {
                Number = nextNumber,
                Side = lastSide,
                Title = "Nowy utwór",
                Duration = TimeSpan.FromMinutes(3)
            }));
        }

        private void DeleteTrack()
        {
            if(SelectedTrack != null)
            {
                Tracks.Remove(SelectedTrack);
            }
        }

        private void MoveTrackUp()
        {
            if (SelectedTrack == null) return;
            int index = Tracks.IndexOf(SelectedTrack);
            MoveAndRenumber(index, index - 1);
        }

        private void MoveTrackDown()
        {
            if (SelectedTrack == null) return;
            int index = Tracks.IndexOf(SelectedTrack);
            MoveAndRenumber(index, index + 1);
        }

        private void MoveAndRenumber(int oldIndex, int newIndex)
        {
            Tracks.Move(oldIndex, newIndex);
            SelectedTrack = Tracks[newIndex];

            for (int i = 0; i < Tracks.Count; i++)
            {
                Tracks[i].Number = i + 1;
            }
            ((RelayCommand)MoveTrackUpCommand).RaiseCanExecuteChanged();
            ((RelayCommand)MoveTrackDownCommand).RaiseCanExecuteChanged();
        }

        private void Save()
        {
            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            int totalMinutes = (int)Math.Floor(ts.TotalMinutes);
            return $"{totalMinutes:00}:{ts.Seconds:00}";
        }

        private void RefreshTotals()
        {
            OnPropertyChanged(nameof(SideADurationFormatted));
            OnPropertyChanged(nameof(SideBDurationFormatted));
            OnPropertyChanged(nameof(TotalDurationFormatted));
            OnPropertyChanged(nameof(TrackCount));
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
