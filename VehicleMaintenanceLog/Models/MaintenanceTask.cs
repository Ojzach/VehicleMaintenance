using System;
using System.Diagnostics;

namespace VehicleMaintenanceLog.Models
{
    public class MaintenanceTask
    {

        public int id;
        public string name;
        public VehicleType vehicleType;
        public string description;


        public MaintenanceTask(int _id = -1, string _name = "", VehicleType _vehicleType = VehicleType.Car, string _description = "")
        {
            id = _id;
            name = _name;
            vehicleType = _vehicleType;
            description = _description;
        }

        public MaintenanceTask(Int64 ID, String TaskName, String VehicleType, String TaskDescription)
        {
            id = (int)ID;
            this.name = TaskName;
            vehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), VehicleType);
            this.description = TaskDescription;

        }

    }
}
