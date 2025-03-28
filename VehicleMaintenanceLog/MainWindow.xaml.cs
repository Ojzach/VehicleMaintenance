using System.CodeDom;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VehicleMaintenanceLog.Classes;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        pMainMenu mainMenu;
        pMaintenanceMenu maintenanceMenu = new pMaintenanceMenu();

        public MainWindow()
        {
            InitializeComponent();

            mainMenu = new pMainMenu();
            SetWindowState(MenuState.VehicleMenu);
        }

        private void SetWindowState(MenuState selectedMenu)
        {
            
            btnBack.IsEnabled = true;
            btnViewLog.IsEnabled = true;
            btnViewTasks.IsEnabled = true;

            mainMenu.ClosePage();
            maintenanceMenu.ClosePage();


            if (selectedMenu == MenuState.VehicleMenu)
            {
                MainPanel.Content = mainMenu;
                mainMenu.LoadPage();
                btnBack.IsEnabled = false;
                txtTitle.Text = "Vehicle Maintenance";
            }
            else if (selectedMenu == MenuState.LogMenu)
            {
                MainPanel.Content = maintenanceMenu;
                maintenanceMenu.LoadPage(MaintenanceMenuStatus.LogMenu, App.selectedVehicleID);
                btnViewLog.IsEnabled = false;
                txtTitle.Text = "Vehicle Maintenance Log";
            }
            else if (selectedMenu == MenuState.TaskMenu)
            {
                MainPanel.Content = maintenanceMenu;
                maintenanceMenu.LoadPage(MaintenanceMenuStatus.TaskMenu, App.selectedVehicleID);
                btnViewTasks.IsEnabled = false;
                txtTitle.Text = "Vehicle Tasks";
            }


        }

        private void btnViewLog_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.LogMenu);
        private void btnBack_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.VehicleMenu);
        private void btnViewTasks_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.TaskMenu);

        private void btnCreateVehicle_Click(object sender, RoutedEventArgs e)
        {
            App.VehicleDataInputWindow.LoadPage();
        }

        private enum MenuState
        {
            Empty = 0,
            VehicleMenu = 1,
            TaskMenu = 2,
            LogMenu = 3
        }
    }
}
