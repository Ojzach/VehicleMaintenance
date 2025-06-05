using VehicleMaintenanceLog.Classes;
using System;

namespace VehicleMaintenanceLog.Models
{
    public class TaskSchedule
    {

        public int id;
        public int taskID;

        public int maintenanceProfileID;
        public int vehicleAssignedTo;

        public int mileageIncrement;
        public int timeIncrement;

        public string scheduleNotes;


        public string MileageIncrement { get { return mileageIncrement == -1 ? "" : mileageIncrement.ToString(); } } //How often this task needs to be performed in miles
        public string TimeIncrement { get { return timeIncrement == -1 ? "" : timeIncrement.ToString(); } } //How often this task needs to be performed in months


        public TaskSchedule(int _taskID = -1, int _maintenanceProfileID = -1, int _assignedVehicleID = -1, int _timeIncrement = -1, int _distanceIncrement = -1, string _notes = "")
        {
            taskID = _taskID;
            maintenanceProfileID = _maintenanceProfileID;
            vehicleAssignedTo = _assignedVehicleID;
            timeIncrement = _timeIncrement;
            mileageIncrement = _distanceIncrement;
            scheduleNotes = _notes;
        }

        public TaskSchedule(Int64 ID, Int64 TaskID, Int64 MaintenanceProfileID, Int64 AssignedVehicleID, Int64 TimeIncrement, Int64 DistanceIncrement, String Notes)
        {
            this.taskID = (int)TaskID;
            id = (int)ID;
            maintenanceProfileID = (int)MaintenanceProfileID;
            vehicleAssignedTo = AssignedVehicleID == 0 ? -1 : (int)AssignedVehicleID;
            timeIncrement = TimeIncrement == 0 ? -1 : (int)TimeIncrement;
            mileageIncrement = DistanceIncrement == 0 ? -1 : (int)DistanceIncrement;
            scheduleNotes = Notes;

        }
    }
}
