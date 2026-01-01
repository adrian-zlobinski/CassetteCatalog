using CassetteCatalog.Wpf.ViewModels;
using System.Windows;

namespace CassetteCatalog.Wpf
{
    /// <summary>
    /// Interaction logic for AlbumEditWindow.xaml
    /// </summary>
    public partial class AlbumEditWindow : Window
    {
        public AlbumEditWindow()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                if (DataContext is AlbumEditViewModel vm)
                {
                    vm.RequestClose += (result) =>
                    {
                        this.DialogResult = result;
                        this.Close();
                    };
                }
            };
        }
    }
}
