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
        #region Variables
        public string Artist { get; set; } = "Nowy Artysta";
        public string Title { get; set; } = "Nowy Tytuł";
        public ushort ReleaseYear { get; set; } = (ushort)DateTime.Now.Year;
        private string _cassettName = "No Name";
        public string CassetteName
        {
            get => _cassettName;
            set
            {
                _cassettName = value;
                OnPropertyChanged();
            }
        }

        private eTapeType _selectedTapeType;
        public eTapeType SelectedTapeType
        {
            get => _selectedTapeType;
            set { _selectedTapeType = value; OnPropertyChanged(); }
        }
        public IEnumerable<eTapeType> TapeTypes => Enum.GetValues(typeof(eTapeType)).Cast<eTapeType>();

        public ObservableCollection<Track> Tracks { get; set; } = new();
        private Track? _selectedTrack;
        public Track? SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();
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
        }

        private bool CanMoveTrackUp() => SelectedTrack != null && Tracks.IndexOf(SelectedTrack) > 0;
        private bool CanMoveTrackDown() => SelectedTrack != null && Tracks.IndexOf(SelectedTrack) < Tracks.Count - 1;
        private void AddTrack()
        {
            int nextNumber = Tracks.Any() ? Tracks.Max(t => t.Number) + 1 : 1;
            var lastSide = Tracks.LastOrDefault()?.Side ?? eCassetteSide.A;
            Tracks.Add(new Track
            {
                Number = nextNumber,
                Side = lastSide,
                Title = "Nowy utwór",
                Duration = TimeSpan.FromMinutes(3)
            });
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

            for(int i = 0; i < Tracks.Count; i++)
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


        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
