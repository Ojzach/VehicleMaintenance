using System;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels.Models
{
    public class TaskScheduleViewModel : ViewModelBase
    {

        public int ScheduleID { get => _taskSchedule.id; }
        public int TaskID { get => _taskSchedule.taskID; set { _taskSchedule.taskID = value; } }

        public int MaintenanceProfileID { get => _taskSchedule.maintenanceProfileID; set => _taskSchedule.maintenanceProfileID = value; }
        public int VehicleAssignedTo { get => _taskSchedule.vehicleAssignedTo; set => _taskSchedule.vehicleAssignedTo = value; }

        public int MileageIncrement { get => _taskSchedule.mileageIncrement; set => _taskSchedule.mileageIncrement = value; }
        public int TimeIncrement { get => _taskSchedule.timeIncrement; set => _taskSchedule.timeIncrement = value; }

        public string MileageIncrementStr { get { return _taskSchedule.mileageIncrement == -1 ? "NA" : _taskSchedule.mileageIncrement.ToString(); } }
        public string TimeIncrementStr { get { return _taskSchedule.timeIncrement == -1 ? "NA" : _taskSchedule.timeIncrement.ToString(); } }

        public string ScheduleNotes { get => _taskSchedule.scheduleNotes; set => _taskSchedule.scheduleNotes = value; }



        public string TaskName { get => SqliteDataAccess.GetValue<string>("MaintenanceTask", _taskSchedule.taskID, "TaskName"); }
        public VehicleType TaskVehicleType { get => (VehicleType)Enum.Parse(typeof(VehicleType), SqliteDataAccess.GetValue<string>("MaintenanceTask", _taskSchedule.taskID, "VehicleType")); }


        private TaskSchedule _taskSchedule;
        public TaskScheduleViewModel(TaskSchedule schedule) 
        {
            _taskSchedule = schedule;
        }

        public void SetID(int id) => _taskSchedule.id = id;
        public TaskSchedule ToTaskSchedule() => _taskSchedule;
    }
}
