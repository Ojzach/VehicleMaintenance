using System;
using System.Windows.Controls;
using VehicleMaintenanceLog.Classes;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pNewTaskDataInput.xaml
    /// </summary>
    public partial class pScheduleDataInput : Page, IDataEntryBoxPage
    {
        public Page InputPage { get => this; }
        private string inputTitle = "New Schedule";
        public string InputTitle { get => inputTitle; }

        
        public (int w, int h) WindowDimensions => (350, 400);

        public ObservableCollection<MaintenanceTask> Tasks { get; set; } = new ObservableCollection<MaintenanceTask>();
        public MaintenanceTask SelectedTask { get; set; }
        public ObservableCollection<Vehicle> Vehicles { get; set; } = new ObservableCollection<Vehicle>();


        private int editModeSelectedSchedule = -1;
        private int defaultvID = -1;


        public pScheduleDataInput()
        {
            DataContext = this;
            InitializeComponent();

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) cbVehicleType.Items.Add(type);
        }

        public void LoadPage(bool editMode = false, int vID = -1, int editItemID = -1)
        {
            ClearData(); 
            
            cbVehicleType.IsEnabled = true;
            cbTask.IsEnabled = true;

            defaultvID = vID;

            if (editMode == true)
            {
                cbVehicleType.IsEnabled = false;
                cbTask.IsEnabled = false;

                TaskSchedule schedule = SqliteDataAccess.GetValue<TaskSchedule>("MaintenanceSchedules", editItemID);
                tbDefaultMileageIncrement.Text = schedule.MileageIncrement;
                tbDefaultTimeIncrement.Text = schedule.TimeIncrement;

                cbVehicleType.SelectedIndex = (int)schedule.TaskVehicleType;

                inputTitle = "Edit Schedule: " + schedule.TaskName;

                editModeSelectedSchedule = editItemID;

                foreach(MaintenanceTask task in Tasks)
                {
                    if (task.TaskID == schedule.TaskID) cbTask.SelectedItem = task;
                }

            }
            else
            {
                cbVehicleType.SelectedIndex = editItemID;
            }

        }

        public void ClosePage()
        {
            ClearData();
        }

        public void ClearData()
        {
            tbDefaultMileageIncrement.Text = string.Empty;
            tbDefaultTimeIncrement.Text = string.Empty;
            tbNotes.Text = string.Empty;
            inputTitle = "New Schedule";

            cbVehicleType.SelectedIndex = -1;
            cbTask.SelectedIndex = -1;
        }

        public bool SubmitData(bool editMode = false)
        {

            if(cbTask.SelectedItem != null)
            {
                int mileageIncrement = tbDefaultMileageIncrement.Text == string.Empty ? -1 : int.Parse(tbDefaultMileageIncrement.Text);
                int timeIncrement = tbDefaultTimeIncrement.Text == string.Empty ? -1 : int.Parse(tbDefaultTimeIncrement.Text);
                
                if (!editMode)
                {
                    if (SqliteDataAccess.CheckMaintenanceTaskExists(SelectedTask.TaskID, ((Vehicle)cbVehicle.SelectedItem).vehicleID))
                    {
                        MessageBox.Show("Schedule Already Exists", "Delete Confirmation", MessageBoxButton.OK);
                    }
                    else
                    {
                        SqliteDataAccess.CreateTaskSchedule(new TaskSchedule(SelectedTask.TaskID, ((Vehicle)cbVehicle.SelectedItem).vehicleID, timeIncrement, mileageIncrement, tbNotes.Text));
                    }
                }
                else
                {
                    SqliteDataAccess.EditMaintenanceSchedule(new TaskSchedule(editModeSelectedSchedule, SelectedTask.TaskID, ((Vehicle)cbVehicle.SelectedItem).vehicleID, timeIncrement, mileageIncrement, tbNotes.Text));
                }


                return true;
            }

            return false;
            
        }

        private void bCreateTask_Click(object sender, RoutedEventArgs e)
        {
            int selectedVehicleType = cbVehicleType.SelectedItem == null ? -1 : (int)cbVehicleType.SelectedItem;
            App.TaskDataInputWindow.LoadPage(selectedVehicleType);
        }

        private void cbVehicleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbVehicleType.SelectedIndex != -1)
            {
                Vehicles.Clear();
                Vehicles.Add(new Vehicle("All", (VehicleType)cbVehicleType.SelectedItem, 0, DateTime.MinValue, id: -1));
                cbVehicle.SelectedIndex = 0;


                foreach (Vehicle v in SqliteDataAccess.LoadVehicles())
                {
                    if (v.vehicleType == (VehicleType)cbVehicleType.SelectedItem)
                    {
                        Vehicles.Add(v);
                        if (v.vehicleID == defaultvID) cbVehicle.SelectedIndex = Vehicles.Count - 1;
                    }
                }

                Tasks.Clear();
                foreach (MaintenanceTask task in SqliteDataAccess.GetTasks((VehicleType)cbVehicleType.SelectedItem)) Tasks.Add(task);
            }

        }

        private void TextIsIntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }


    }
}
