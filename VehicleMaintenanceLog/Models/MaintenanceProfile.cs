using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleMaintenanceLog.Models
{
    public class MaintenanceProfile
    {

        public int id;
        public string name;
        public VehicleType vehicleTypeConstraint;

        public MaintenanceProfile(int _id = -1, string _name = "", VehicleType _vehicleTypeConstraint = default)
        {
            id = _id;
            name = _name;
            vehicleTypeConstraint = _vehicleTypeConstraint;
        }

        public MaintenanceProfile(Int64 ID, String ProfileName)
        {
            id = (int)ID;
            this.name = ProfileName;

        }
    }
}
