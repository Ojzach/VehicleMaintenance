using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.ViewModels.Models;
using VehicleMaintenanceLog.Views.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow
{
    class MaintenanceProfileDataInputViewModel : ViewModelBase, IDataEntryView
    {
        private MaintenanceProfileDataInputView _inputView;
        public UserControl InputView { get => _inputView; }

        public string InputTitle => _isEditMode ? "Edit Maintenance Profile" : "New Maintenance Profile";

        public (int w, int h) WindowDimensions => (350, 350);


        private bool _isEditMode;

        private ObservableCollection<VehicleType> _vehicleTypes = new ObservableCollection<VehicleType>();
        public ObservableCollection<VehicleType> VehicleTypes { get => _vehicleTypes; set { _vehicleTypes = value; OnPropertyChanged("VehicleTypes"); } }


        private MaintenanceProfileViewModel _newProfile;
        public MaintenanceProfileViewModel NewProfile
        {
            get => _newProfile;
            set
            {
                _newProfile = value;
                OnPropertyChanged("NewProfile");
            }
        }

        public MaintenanceProfileDataInputViewModel()
        {
            _inputView = new MaintenanceProfileDataInputView { DataContext = this };

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) VehicleTypes.Add(type);
        }


        public bool CanSubmit()
        {
            return (NewProfile != null && NewProfile.ProfileName != "");
        }

        public void ClearInputs() => NewProfile = new MaintenanceProfileViewModel(new MaintenanceProfile());

        public void LoadPage(bool editMode, ViewModelBase selectedItemToEdit)
        {
            if (editMode) NewProfile = (MaintenanceProfileViewModel)selectedItemToEdit;
            else NewProfile = new MaintenanceProfileViewModel(new MaintenanceProfile());

            _isEditMode = editMode;
            OnPropertyChanged("InputTitle");
        }

        public object SubmitData()
        {
            if (!_isEditMode)
            {
                SqliteDataAccess.CreateMaintenanceProfile(NewProfile.ToMaintenanceProfile());
                NewProfile.SetID(SqliteDataAccess.GetNewestProfileID());
            }
            else SqliteDataAccess.EditMaintenanceProfile(NewProfile.ToMaintenanceProfile());

            return NewProfile;
        }
    }
}
