using System;
using System.Diagnostics;

namespace VehicleMaintenanceLog.Classes
{
    public class MaintenanceTask
    {

        public int TaskID { get; set; } = -1;
        public string TaskName { get; set; } = "";

        public VehicleType vehicleType;
        public string TaskDescription { get; set; } = "";


        public MaintenanceTask(string TaskName, VehicleType VehicleType, string TaskDescription)
        {
            this.TaskName = TaskName;
            vehicleType = VehicleType;
            this.TaskDescription = TaskDescription;

        }

        public MaintenanceTask(int ID, string TaskName, VehicleType VehicleType, string TaskDescription)
        {
            TaskID = ID;
            this.TaskName = TaskName;
            vehicleType = VehicleType;
            this.TaskDescription = TaskDescription;
        }

        public MaintenanceTask(Int64 ID, String TaskName, String VehicleType, String TaskDescription)
        {
            TaskID = (int)ID;
            this.TaskName = TaskName;
            vehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), VehicleType);
            this.TaskDescription = TaskDescription;

        }

    }
}
