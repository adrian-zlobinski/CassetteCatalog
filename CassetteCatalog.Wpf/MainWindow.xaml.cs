using CassetteCatalog.Data;
using CassetteCatalog.Wpf.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CassetteCatalog.Wpf
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            var viewModel = (MainViewModel)this.DataContext;

            if (item?.DataContext is AlbumNode albumNode)
            {
                // Przypisujemy model Album do ViewModelu, co aktywuje przyciski Edytuj/Usuń
                viewModel.SelectedAlbum = albumNode.Album;
            }
            else
            {
                // Jeśli kliknięto Artystę, czyścimy wybór albumu
                viewModel.SelectedAlbum = null;
            }

            // Zapobiega wywoływaniu zdarzenia dla rodziców w górę drzewa
            e.Handled = true;
        }
    }
}