using VehicleMaintenanceLog.Classes;
using System;

namespace VehicleMaintenanceLog.Models
{
    public class MaintenanceTaskSchedule
    {

        public int ScheduleID { get; set; } = -1;
        public int TaskID { get; } = -1;

        public int maintenanceProfileID = -1;
        public int vehicleAssignedTo = -1;

        public int mileageIncrement = -1;
        public int timeIncrement = -1;

        public string ScheduleNotes { get; set; } = "";


        public string MileageIncrement { get { return mileageIncrement == -1 ? "" : mileageIncrement.ToString(); } } //How often this task needs to be performed in miles
        public string TimeIncrement { get { return timeIncrement == -1 ? "" : timeIncrement.ToString(); } } //How often this task needs to be performed in months


        public MaintenanceTaskSchedule(int _taskID, int _maintenanceProfileID, int _assignedVehicleID, int _timeIncrement, int _distanceIncrement, string _notes)
        {
            TaskID = _taskID;
            maintenanceProfileID = _maintenanceProfileID;
            vehicleAssignedTo = _assignedVehicleID;
            timeIncrement = _timeIncrement;
            mileageIncrement = _distanceIncrement;
            ScheduleNotes = _notes;
        }
        public MaintenanceTaskSchedule(Int64 ID, Int64 TaskID, Int64 MaintenanceProfileID, Int64 AssignedVehicleID, Int64 TimeIncrement, Int64 DistanceIncrement, String Notes)
        {
            this.TaskID = (int)TaskID;
            ScheduleID = (int)ID;
            maintenanceProfileID = (int)MaintenanceProfileID;
            vehicleAssignedTo = AssignedVehicleID == 0 ? -1 : (int)AssignedVehicleID;
            timeIncrement = TimeIncrement == 0 ? -1 : (int)TimeIncrement;
            mileageIncrement = DistanceIncrement == 0 ? -1 : (int)DistanceIncrement;
            ScheduleNotes = Notes;

        }

        public string TaskName { get => SqliteDataAccess.GetValue<string>("MaintenanceTask", TaskID, "TaskName"); }
        public VehicleType TaskVehicleType { get => (VehicleType)Enum.Parse(typeof(VehicleType), SqliteDataAccess.GetValue<string>("MaintenanceTask", TaskID, "VehicleType")); }
    }
}
