using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels.Models
{
    public class MaintenanceProfileViewModel : ViewModelBase
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

        public VehicleType VehicleTypeConstraint
        {
            get => _profile.vehicleTypeConstraint;
            set
            {
                _profile.vehicleTypeConstraint = value;
            }
        }


        private readonly MaintenanceProfile _profile;
        public MaintenanceProfileViewModel(MaintenanceProfile profile)
        {
            _profile = profile;
        }

        public void SetID(int id) => _profile.id = id;
        public MaintenanceProfile ToMaintenanceProfile() => _profile;

        public override bool Equals(object obj)
        {
            if (obj is MaintenanceProfileViewModel vm) return ((MaintenanceProfileViewModel)obj).ProfileID == ProfileID;
            else return false;
        }
        public override int GetHashCode() => base.GetHashCode();

    }
}
