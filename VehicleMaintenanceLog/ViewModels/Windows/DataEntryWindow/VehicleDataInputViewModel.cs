using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.ViewModels.Models;
using VehicleMaintenanceLog.Views.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow
{
    class VehicleDataInputViewModel : ViewModelBase, IDataEntryView
    {
        private VehicleDataInputView _inputView;
        public UserControl InputView { get => _inputView; }

        public string InputTitle => _isEditMode ? "Edit Vehicle" : "New Vehicle";

        public (int w, int h) WindowDimensions => (350, 350);
        private bool _isEditMode;

        private ObservableCollection<VehicleType> _vehicleTypes = new ObservableCollection<VehicleType>();
        private ObservableCollection<MaintenanceProfileViewModel> _maintenanceProfiles = new ObservableCollection<MaintenanceProfileViewModel>();
        public ObservableCollection<VehicleType> VehicleTypes { get => _vehicleTypes; set { _vehicleTypes = value; OnPropertyChanged("VehicleTypes"); } }
        public ObservableCollection<MaintenanceProfileViewModel> MaintenanceProfiles { get => _maintenanceProfiles; set { _maintenanceProfiles = value; OnPropertyChanged("MaintenanceProfiles"); } }


        private VehicleViewModel _newVehicle;
        public VehicleViewModel NewVehicle
        {
            get => _newVehicle;
            set
            {
                _newVehicle = value;
                OnPropertyChanged(nameof(NewVehicle));
            }
        }

        public VehicleDataInputViewModel()
        {
            _inputView = new VehicleDataInputView() { DataContext = this };

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) VehicleTypes.Add(type);
        }

        public bool CanSubmit()
        {
            return (NewVehicle != null && NewVehicle.VehicleName != "" && NewVehicle.VehicleMileage >= 0);
        }

        public void ClearInputs() => NewVehicle = new VehicleViewModel(new Vehicle(_maintenanceProfileID: 0));

        public void LoadPage(bool editMode, ViewModelBase selectedItemToEdit, object inputData)
        {
            if (editMode) NewVehicle = (VehicleViewModel)selectedItemToEdit;
            else ClearInputs();

            _isEditMode = editMode;
            OnPropertyChanged("InputTitle");

            MaintenanceProfiles.Clear();
            foreach (MaintenanceProfile profile in SqliteDataAccess.GetAllOfType<MaintenanceProfile>())
            {
                MaintenanceProfiles.Add(new MaintenanceProfileViewModel(profile));
            }
        }

        public object SubmitData()
        {
            if (!_isEditMode)
            {
                SqliteDataAccess.CreateItem<Vehicle>(NewVehicle.ToVehicle());
                NewVehicle.SetID(SqliteDataAccess.GetNewestItemID<Vehicle>());
            }
            else SqliteDataAccess.EditItem<Vehicle>(NewVehicle.ToVehicle());

            return NewVehicle;
        }
    }
}
