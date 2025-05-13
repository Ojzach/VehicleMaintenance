using System;
using VehicleMaintenanceLog.Classes;

namespace VehicleMaintenanceLog.Models
{
    public class MaintenanceLogItem
    {

        public int LogID { get; set; } = -1;
        public int VehicleID { get; private set; }
        public string TaskName { get { return SqliteDataAccess.GetValue<string>("MaintenanceTask", taskID, "TaskName"); } }
        public string DateCompleted { get { return datecompleted.ToString("d"); } }
        public string LogNotes { get; private set; } = "";

        public int taskID;
        public int VehicleMileage { get; private set; } = -1;
        public DateTime datecompleted = DateTime.MinValue;



        public int tempFixMileage = -1;
        public int tempFixTime = -1;
        public string TempFixMileage { get { return tempFixMileage == -1 ? "" : tempFixMileage.ToString(); } } //How often this task needs to be performed in miles

        public string TempFixTime { get { return tempFixTime == -1 ? "" : tempFixTime.ToString(); } } //How often this task needs to be performed in months
        public string TempFixTxt { get
            {
                if(tempFixMileage == -1 && tempFixTime == -1)
                {
                    return "NA";
                }
                else
                {
                    return "M: " + TempFixMileage + " T: " + TempFixTime;
                }
            } }


        public MaintenanceLogItem(int _vehicleID, int _taskID, int _mileageCompleted, DateTime _timeCompleted, int _tempFixMileage, int _tempFixTime, string _logNotes = "")
        {
            VehicleID = _vehicleID;
            taskID = _taskID;
            VehicleMileage = _mileageCompleted;
            datecompleted = _timeCompleted;
            LogNotes = _logNotes;
            tempFixMileage = _tempFixMileage;
            tempFixTime = _tempFixTime;
        }

        public MaintenanceLogItem(Int64 ID, Int64 VehicleID, Int64 TaskID, Int64 VehicleMileage, Int64 DateCompleted, Int64 TempFixMileage, Int64 TempFixTime, String Notes)
        {
            LogID = (int)ID;
            this.VehicleID = (int)VehicleID;
            taskID = (int)TaskID;
            this.VehicleMileage = (int)VehicleMileage;
            datecompleted = DateTime.FromBinary(DateCompleted);
            LogNotes = Notes;
            tempFixMileage = TempFixMileage == 0 ? -1 : (int)TempFixMileage;
            tempFixTime = TempFixTime == 0 ? -1 : (int)TempFixTime;
        }


        public override bool Equals(object obj)
        {
            if (obj is MaintenanceLogItem)
            {
                MaintenanceLogItem log = (MaintenanceLogItem)obj;

                if(log.LogID == LogID) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
