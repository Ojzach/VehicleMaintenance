using System;
using System.Windows.Controls;
using VehicleMaintenanceLog.Classes;
using System.Windows.Input;
using System.Linq;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pNewTaskDataInput.xaml
    /// </summary>
    public partial class pTaskDataInput : Page, IDataEntryBoxPage
    {
        public Page InputPage { get => this; }
        private string inputTitle = "New Task";
        public string InputTitle { get => inputTitle; }

        
        public (int w, int h) WindowDimensions => (350, 350);


        private int currentSelectedVehicle = -1;
        private int currentSelectedSchedule = -1;

        public pTaskDataInput()
        {
            InitializeComponent();

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) cbVehicleType.Items.Add(type);
        }

        public void LoadPage(bool editMode = false, int vID = -1, int editItemID = -1) //If editMode == true vID is the vID if false it is the selected vehicleType
        {
            ClearData();

            tbDefaultTimeIncrement.Visibility = System.Windows.Visibility.Visible;
            tbDefaultMileageIncrement.Visibility = System.Windows.Visibility.Visible;
            txtDefaultSchedule.Visibility = System.Windows.Visibility.Visible;

            if (editMode == true)
            {
                tbDefaultTimeIncrement.Visibility = System.Windows.Visibility.Hidden;
                tbDefaultMileageIncrement.Visibility = System.Windows.Visibility.Hidden;
                txtDefaultSchedule.Visibility = System.Windows.Visibility.Hidden;

                TaskSchedule schedule = SqliteDataAccess.GetValue<TaskSchedule>("MaintenanceSchedules", editItemID);
                tbName.Text = schedule.TaskName;

                cbVehicleType.SelectedIndex = (int)schedule.TaskVehicleType;

                inputTitle = "Edit Task: " + schedule.TaskName;

                currentSelectedSchedule = editItemID;
                currentSelectedVehicle = vID;
            }
            else cbVehicleType.SelectedIndex = vID;

        }

        public void ClosePage()
        {
            ClearData();
        }

        public void ClearData()
        {
            tbName.Text = string.Empty;
            tbDefaultMileageIncrement.Text = string.Empty;
            tbDefaultTimeIncrement.Text = string.Empty;
            inputTitle = "New Task";
        }

        public bool SubmitData(bool editMode = false)
        {
            if (cbVehicleType.SelectedItem != null && tbName.Text != "")
            {
                int mileageIncrement = tbDefaultMileageIncrement.Text == string.Empty ? -1 : int.Parse(tbDefaultMileageIncrement.Text);
                int timeIncrement = tbDefaultTimeIncrement.Text == string.Empty ? -1 : int.Parse(tbDefaultTimeIncrement.Text);

                if (!editMode)
                {
                    SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask(tbName.Text, (VehicleType)cbVehicleType.SelectedItem, ""));
                    SqliteDataAccess.CreateTaskSchedule(new TaskSchedule(SqliteDataAccess.GetNewestTaskID(), -1, timeIncrement, mileageIncrement, ""));
                }
                else
                {
                    SqliteDataAccess.EditTask(new MaintenanceTask(currentSelectedSchedule, tbName.Text, (VehicleType)currentSelectedVehicle, ""));
                }

                return true;
            }

            return false;
        }

        private void TextIsIntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }

    }
}
