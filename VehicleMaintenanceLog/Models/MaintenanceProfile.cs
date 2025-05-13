using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleMaintenanceLog.Models
{
    class MaintenanceProfile
    {

        public int id;
        public string name;

        public MaintenanceProfile(int _id = -1, string _name = "")
        {
            id = _id;
            name = _name;
        }

        public MaintenanceProfile(Int64 ID, String ProfileName)
        {
            id = (int)ID;
            this.name = ProfileName;

        }
    }
}
