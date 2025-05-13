using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels.Models
{
    class MaintenanceProfileViewModel : ViewModelBase
    {

        public int ProfileID { get => _profile.id; }
        public string ProfileName
        {
            get => _profile.name;
            set
            {
                _profile.name = value;
            }
        }

        private readonly MaintenanceProfile _profile;
        public MaintenanceProfileViewModel(MaintenanceProfile profile)
        {
            _profile = profile;
        }

        public void SetID(int id) => _profile.id = id;
        public MaintenanceProfile ToMaintenanceProfile() => _profile;
    }
}
