using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels
{
    class MaintenanceTaskViewModel : ViewModelBase
    {

        public int TaskID { get => _maintenanceTask.id; }
        public string TaskName { 
            get => _maintenanceTask.name; 
            set
            {
                _maintenanceTask.name = value;
            }
        }
        public VehicleType TaskVehicleType { 
            get => _maintenanceTask.vehicleType; 
            set
            {
                _maintenanceTask.vehicleType = value;
            }
        }
        public string TaskDescription { 
            get => _maintenanceTask.description;
            set
            {
                _maintenanceTask.description = value;
            }
        }

        private readonly MaintenanceTask _maintenanceTask;

        public MaintenanceTaskViewModel(MaintenanceTask maintenanceTask)
        {
            _maintenanceTask = maintenanceTask;
        }

        public void SetID(int id) => _maintenanceTask.id = id;
        public MaintenanceTask ToMaintenanceTask() => _maintenanceTask;
    }
}
