using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VehicleMaintenanceLog.Classes;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pNewLogDataInput.xaml
    /// </summary>
    public partial class pLogDataInput : Page, IDataEntryBoxPage
    {

        public Page InputPage { get => this; }

        private string inputTitle = "New Log";
        public string InputTitle { get => inputTitle; }

        public (int w, int h) WindowDimensions => (450, 450);

        public ObservableCollection<MaintenanceTask> Tasks { get; set; } = new ObservableCollection<MaintenanceTask>();
        public MaintenanceTask SelectedTask { get; set; }

        public ObservableCollection<Vehicle> Vehicles { get; set; } = new ObservableCollection<Vehicle>();


        private int editLogID = -1;
        private int currentSelectedVehicle = -1;

        public pLogDataInput()
        {
            DataContext = this;
            InitializeComponent();
        }


        public void LoadPage(bool editMode = false, int vID = -1, int editItemID = -1)
        {
            ClearData();

            editLogID = editItemID;
            currentSelectedVehicle = vID;

            if (vID != -1) LoadTaskComboBox();

            foreach (Vehicle v in SqliteDataAccess.LoadVehicles()) Vehicles.Add(v);
            cbVehicle.SelectedItem = Vehicles?.Where(x => x.vehicleID == vID).First();


            if(editMode == true)
            {
                MaintenanceLogItem log = SqliteDataAccess.GetValue<MaintenanceLogItem>("MaintenanceLog", editItemID);
                cbVehicle.IsEnabled = false;

                cbTask.SelectedItem = Tasks?.Where(x => x.TaskID == log.taskID).First();
                tbMileage.Text = log.VehicleMileage.ToString();
                dpDate.SelectedDate = log.datecompleted;
                tbNotes.Text = log.LogNotes;
                
                if(log.TempFixMileage != "" || log.TempFixTime != "")
                {
                    chbTempFix.IsChecked = true;
                    tbTempFixMiles.Text = log.TempFixMileage;
                    tbTempFixTime.Text = log.TempFixTime;
                }

                tbTempFixTime.Text = log.TempFixTime.ToString();
                tbTempFixMiles.Text = log.TempFixMileage.ToString();

                inputTitle = "Edit Log: " + log.TaskName;
            }

            App.TaskDataInputWindow.NewTaskCreated += LoadTaskComboBox;
        }

        public void ClosePage()
        {
            Vehicles.Clear();
            Tasks.Clear();

            ClearData();

            App.TaskDataInputWindow.NewTaskCreated -= LoadTaskComboBox;
        }

        public void ClearData()
        {
            SelectedTask = null;
            cbTask.SelectedItem = null;
            tbMileage.Text = "";
            tbNotes.Text = "";
            tbTempFixMiles.Text = "";
            tbTempFixTime.Text = "";
            chbTempFix.IsChecked = false;
            spTempFixInputs.Visibility = Visibility.Collapsed;
            dpDate.SelectedDate = DateTime.Today;       
        }

        public bool SubmitData(bool editMode = false)
        {
            if (cbTask.SelectedItem != null && tbMileage.Text != "" && dpDate.SelectedDate != null)
            {

                if (SqliteDataAccess.GetValue<Vehicle>("Vehicle", currentSelectedVehicle).vehicleMileage < int.Parse(tbMileage.Text))
                {
                    
                    MessageBoxResult result = MessageBox.Show("Log Mileage Greater Than Vehicle Mileage. Do You Want To Update Vehicle Mileage?", "", MessageBoxButton.YesNoCancel);

                    if (result == MessageBoxResult.Yes) SqliteDataAccess.SetVehicleMileage(currentSelectedVehicle, int.Parse(tbMileage.Text));
                    else if (result == MessageBoxResult.Cancel) return false;
                }

                int tempFixMiles = -1, tempFixTime = -1;
                if(chbTempFix.IsChecked == true)
                {
                    tempFixMiles = tbTempFixMiles.Text == string.Empty ? -1 : int.Parse(tbTempFixMiles.Text);
                    tempFixTime = tbTempFixTime.Text == string.Empty ? -1 : int.Parse(tbTempFixTime.Text);
                }
                

                MaintenanceLogItem log = new MaintenanceLogItem(currentSelectedVehicle, SelectedTask.TaskID, int.Parse(tbMileage.Text), (DateTime)dpDate.SelectedDate, tempFixMiles, tempFixTime, tbNotes.Text);

                if (!editMode) SqliteDataAccess.CreateMaintenanceLog(log);
                else SqliteDataAccess.EditMaintenanceLog(editLogID, log);

                return true;
            }

            return false;
        }

        public void LoadTaskComboBox()
        {
            Tasks.Clear();

            VehicleType selectedType = SqliteDataAccess.GetVehicleType(currentSelectedVehicle);
            foreach (MaintenanceTask t in SqliteDataAccess.LoadMaintenaceTasks(selectedType)) Tasks.Add(t);

        }

        private void cbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVehicle.SelectedItem != null)
            {
                currentSelectedVehicle = ((Vehicle)cbVehicle.SelectedItem).vehicleID;
                LoadTaskComboBox();
                tbMileage.Text = ((Vehicle)cbVehicle.SelectedItem).vehicleMileage.ToString();
            }
        }


        private void chbTempFix_Checked(object sender, RoutedEventArgs e) => spTempFixInputs.Visibility = Visibility.Visible;
        private void chbTempFix_Unchecked(object sender, RoutedEventArgs e) => spTempFixInputs.Visibility = Visibility.Collapsed;



        private void bCreateTask_Click(object sender, RoutedEventArgs e)
        {
            int selectedVehicleType = currentSelectedVehicle == -1 ? -1 : (int)SqliteDataAccess.GetVehicleType(currentSelectedVehicle);
            App.TaskDataInputWindow.LoadPage(selectedVehicleType);
        }

        private void TextIsIntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }

    }
}
