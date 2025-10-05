using System;
using System.Diagnostics;
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new VehicleOverviewPageViewModel();

            MainWindow = new MainWindow() { DataContext = new MainViewModel(_navigationStore) };
            MainWindow.Show();

            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogCrashDetails(ex, "Unhandled exception in application domain.");
            Environment.Exit(1); // Terminate the application
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the exception information
            LogCrashDetails(e.Exception, "Unhandled exception on UI thread.");

            e.Handled = true;

        }

        private void LogCrashDetails(Exception ex, string message)
        {
            System.IO.File.AppendAllText("crashlog-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt", $"{DateTime.Now}: {message}\n{ex?.ToString()}\n\n");
        }
    }
}
