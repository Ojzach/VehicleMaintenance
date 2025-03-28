using System;
using System.Windows.Controls;
using System.Windows.Input;
using VehicleMaintenanceLog.Classes;
using System.Linq;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pNewVehicleDataInput.xaml
    /// </summary>
    public partial class pVehicleDataInput : Page, IDataEntryBoxPage
    {
        public Page InputPage { get => this; }

        private string inputTitle = "New Vehicle";
        public string InputTitle { get => inputTitle; }
        public (int w, int h) WindowDimensions => (350, 350);
        
        private int editVID = -1;

        public pVehicleDataInput()
        {
            InitializeComponent();

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) cbVehicleType.Items.Add(type);
        }

        public void LoadPage(bool editData = false, int vID = -1, int editItemID = -1) 
        {
            ClearData();
            editVID = vID;

            if(editData == true && vID != -1)
            {
                Vehicle vehicle = SqliteDataAccess.GetValue<Vehicle>("Vehicle", editVID);
                tbName.Text = vehicle.VehicleName;
                cbVehicleType.SelectedIndex = (int)vehicle.vehicleType;
                tbVehicleMileage.Text = vehicle.vehicleMileage.ToString();
                dpManufactureDate.SelectedDate = vehicle.manufactureDate;

                inputTitle = "Edit: " + vehicle.VehicleName;
            }
        }

        public void ClosePage()
        {
            ClearData();
        }

        public void ClearData()
        {
            inputTitle = "New Vehicle";
            tbName.Text = string.Empty;
            tbVehicleMileage.Text = string.Empty;
            dpManufactureDate.SelectedDate = DateTime.Today;
        }

        public bool SubmitData(bool editMode = false)
        {
            if (cbVehicleType.SelectedItem != null && tbName.Text != "" && tbVehicleMileage.Text != "" && dpManufactureDate.SelectedDate != null)
            {
                Vehicle v = new Vehicle(tbName.Text, (VehicleType)cbVehicleType.SelectedItem, int.Parse(tbVehicleMileage.Text), (DateTime)dpManufactureDate.SelectedDate, editVID);

                if (editMode == false) SqliteDataAccess.CreateVehicle(v);
                else SqliteDataAccess.EditVehicle(v);

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
