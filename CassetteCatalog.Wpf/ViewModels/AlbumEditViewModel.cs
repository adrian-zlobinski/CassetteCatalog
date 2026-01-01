using CassetteCatalog.Core.Enums;
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
        public string CassetteName { get; set; } = "No Name";

        private eTapeType _selectedTapeType;
        public eTapeType SelectedTapeType
        {
            get => _selectedTapeType;
            set { _selectedTapeType = value; OnPropertyChanged(); }
        }
        public IEnumerable<eTapeType> TapeTypes => Enum.GetValues(typeof(eTapeType)).Cast<eTapeType>();
        #endregion

        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<bool>? RequestClose;
        #endregion

        public AlbumEditViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
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
