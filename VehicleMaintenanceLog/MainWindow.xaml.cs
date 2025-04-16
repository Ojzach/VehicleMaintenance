using System.Windows;
using VehicleMaintenanceLog.Menus;


namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        pMainMenu mainMenu;
        pMaintenanceMenu maintenanceMenu = new pMaintenanceMenu();
        pGenerateReportMenu reportMenu = new pGenerateReportMenu();

        public MainWindow()
        {
            InitializeComponent();

            mainMenu = new pMainMenu();
            SetWindowState(MenuState.VehicleMenu);
        }

        private void SetWindowState(MenuState selectedMenu)
        {
            
            btnMainMenu.IsEnabled = true;
            btnViewLog.IsEnabled = true;
            btnViewTasks.IsEnabled = true;
            btnMaintenanceReport.IsEnabled = true;

            mainMenu.ClosePage();
            maintenanceMenu.ClosePage();


            if (selectedMenu == MenuState.VehicleMenu)
            {
                MainPanel.Content = mainMenu;
                mainMenu.LoadPage();
                btnMainMenu.IsEnabled = false;
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
            else if(selectedMenu == MenuState.ReportMenu)
            {
                MainPanel.Content = reportMenu;
                reportMenu.LoadPage();
                btnMaintenanceReport.IsEnabled = false;
                txtTitle.Text = "Generate Report";
            }

                
        }

        private void btnViewLog_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.LogMenu);
        private void btnMainMenu_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.VehicleMenu);
        private void btnViewTasks_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.TaskMenu);
        private void btnMaintenanceReport_Click(object sender, RoutedEventArgs e) => SetWindowState(MenuState.ReportMenu);

        private enum MenuState
        {
            Empty = 0,
            VehicleMenu = 1,
            TaskMenu = 2,
            LogMenu = 3,
            ReportMenu = 4
        }
    }
}
