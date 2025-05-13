using System.Windows;
using VehicleMaintenanceLog.Stores;
using VehicleMaintenanceLog.ViewModels;



namespace VehicleMaintenanceLog
{



    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly NavigationStore _navigationStore;

        public App()
        {
            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new VehicleOverviewPageViewModel();

            MainWindow = new MainWindow() { DataContext = new MainViewModel(_navigationStore) };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
