using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VehicleMaintenanceLog.Classes;


namespace VehicleMaintenanceLog.Menus
{
    /// <summary>
    /// Interaction logic for pGenerateReportMenu.xaml
    /// </summary>
    /// 

    

    public partial class pGenerateReportMenu : Page
    {

        public ObservableCollection<Vehicle> Vehicles { get; set; } = new ObservableCollection<Vehicle>();

        public pGenerateReportMenu()
        {
            InitializeComponent();
            DataContext = this;

            cbReportType.Items.Add(new ComboBoxItem() { Content = "Vehicle Maintenance Log" });
        }

        public void LoadPage()
        {
            ClearPage();

            foreach (Vehicle v in SqliteDataAccess.LoadVehicles()) Vehicles.Add(v);
            cbVehicle.SelectedItem = Vehicles?.Where(x => x.vehicleID == App.selectedVehicleID).First();


        }

        public void ClosePage()
        {
            ClearPage();

            Vehicles.Clear();
        }

        public void ClearPage()
        {
            cbReportType.SelectedIndex = 0;
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            GenerateVehicleLogReport(App.selectedVehicleID);
        }

        private void GenerateVehicleLogReport(int vID)
        {
        }
    }
}
