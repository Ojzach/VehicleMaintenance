using System;
using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels
{
    class MaintenanceLogViewModel : ViewModelBase
    {

        public int LogID { get => _maintenanceLog.LogID; }
        public int LogTaskID { get => _maintenanceLog.taskID; }
        public string LogTaskName { get => _maintenanceLog.TaskName; }
        public int LogVehicleMileage { get => _maintenanceLog.VehicleMileage; }
        public DateTime LogDateCompleted { get => _maintenanceLog.datecompleted; }
        public string LogTempFixString
        {
            get
            {
                if (_maintenanceLog.tempFixMileage == -1 && _maintenanceLog.tempFixTime == -1) return "NA";
                else
                {
                    return "M: " + (_maintenanceLog.tempFixMileage == -1 ? "" : _maintenanceLog.tempFixMileage)
                        + " T: " + (_maintenanceLog.tempFixTime == -1 ? "" : _maintenanceLog.tempFixTime);
                }
            }
        }
        public int LogTempFixMileage { get => _maintenanceLog.tempFixMileage; }
        public int LogTempFixTime { get => _maintenanceLog.tempFixTime; }
        public string LogNotes { get => _maintenanceLog.LogNotes; }

        private readonly MaintenanceLogItem _maintenanceLog;

        public MaintenanceLogViewModel(MaintenanceLogItem maintenanceLog)
        {
            _maintenanceLog = maintenanceLog;
        }


        public override bool Equals(object obj)
        {
            if (obj is MaintenanceLogViewModel) return LogID == ((MaintenanceLogViewModel)obj).LogID;
            else return base.Equals(obj);
        }
        public override int GetHashCode() => base.GetHashCode();

    }
}
