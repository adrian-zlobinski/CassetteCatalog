using CassetteCatalog.Wpf.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;


namespace CassetteCatalog.Wpf
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            var appData = Path.Combine(Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData), "CassetteCatalog");

            Directory.CreateDirectory(appData);

            var dbPath = Path.Combine(appData, "CassetteCatalog.db");

            services.AddDbContext<Data.AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"), ServiceLifetime.Singleton);

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }
    }

}
