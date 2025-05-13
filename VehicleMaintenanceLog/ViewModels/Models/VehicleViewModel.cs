using System;
using System.Data.SqlTypes;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels
{
    class VehicleViewModel : ViewModelBase
    {

        public int VehicleID { get => _vehicle.id; }
        public string VehicleName { 
            get => _vehicle.name;
            set
            {
                _vehicle.name = value;
            }
        }
        public VehicleType VehicleType { 
            get =>  _vehicle.type; 
            set
            {
                _vehicle.type = value;
            }
        }
        public int VehicleMileage { 
            get => _vehicle.mileage;
            set {
                _vehicle.mileage = value;
                OnPropertyChanged("VehicleMileage");
            }
        }
        public DateTime VehicleManufactureDate { 
            get => _vehicle.manufactureDate; 
            set
            {
                _vehicle.manufactureDate = value;
            }
        }
        public string VehicleMaintenanceProfile { 
            get => "";
        }


        private readonly Vehicle _vehicle;

        public VehicleViewModel(Vehicle v)
        {
            _vehicle = v;
        }

        public Vehicle ToVehicle() => _vehicle;
        public void SetID(int id) => _vehicle.id = id;

        public void UpdateVehicleMileage(int _mileage)
        {
            VehicleMileage = _mileage;
            SqliteDataAccess.SetVehicleMileage(_vehicle.id, _mileage);
        }
    }
}
