using System;
using System.Diagnostics;
using System.Windows.Media;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels
{
    public class MaintenanceTaskStatusViewModel : ViewModelBase
    {
        public string TaskName { get; set; } = "";
        public string MileageToNext { get { return mileageToNext == int.MaxValue ? "NA" : mileageToNext.ToString(); } }
        public string DateOfNext { get { return dateOfNext == DateTime.MaxValue ? "NA" : dateOfNext.ToString("d"); } }

        public Brush MileageColor
        {
            get
            {
                if (mileageToNext < 0) return redBrush;
                else if (previousLogTempFix.miles == true) return yellowBrush;
                else if (mileageToNext == int.MaxValue) return Brushes.Black;
                else return greenBrush;
            }
        }
        public Brush DateColor
        {
            get
            {
                if (dateOfNext.Ticks - DateTime.Today.Ticks < 0) return redBrush;
                else if (previousLogTempFix.time == true) return yellowBrush;
                else if (dateOfNext == DateTime.MaxValue) return Brushes.Black;
                else return greenBrush;
            }
        }


        private int mileageToNext = int.MaxValue;
        private DateTime dateOfNext = DateTime.MaxValue;
        public (bool miles, bool time) previousLogTempFix = (false, false);


        public int SortByMileage { get => mileageToNext; }
        public string SortByDate { get => dateOfNext.ToString("s"); }


        Brush greenBrush = new SolidColorBrush(Color.FromRgb(33, 166, 27));
        Brush redBrush = new SolidColorBrush(Color.FromRgb(214, 18, 13));
        Brush yellowBrush = new SolidColorBrush(Color.FromRgb(219, 189, 7));

        public MaintenanceTaskStatusViewModel(Vehicle vehicle, TaskSchedule schedule)
        {
            TaskName = SqliteDataAccess.GetValue<string>("MaintenanceTask", schedule.taskID, "TaskName"); ;

            MaintenanceLogItem previousLog = SqliteDataAccess.GetMostRecentVehicleMaintenanceLog(vehicle.id, schedule.taskID);

            if (previousLog != null)
            {
                previousLogTempFix = (previousLog.tempFixMileage != -1, previousLog.tempFixTime != -1);

                if (previousLog.tempFixMileage != -1) mileageToNext = (previousLog.VehicleMileage + previousLog.tempFixMileage) - vehicle.mileage;
                else if (schedule.mileageIncrement != -1) mileageToNext = (previousLog.VehicleMileage + schedule.mileageIncrement) - vehicle.mileage;

                if (previousLog.tempFixTime != -1) dateOfNext = previousLog.datecompleted.AddMonths(previousLog.tempFixTime);
                else dateOfNext = previousLog.datecompleted.AddMonths(schedule.timeIncrement);
            }
            else
            {
                if(schedule.mileageIncrement != -1) mileageToNext = schedule.mileageIncrement - vehicle.mileage;
                if(schedule.timeIncrement != -1) dateOfNext = vehicle.manufactureDate.AddMonths(schedule.timeIncrement);
            }
        }
    }
}
