using System;
using System.Windows;
using VehicleMaintenanceLog.Classes;



namespace VehicleMaintenanceLog
{



    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static int selectedVehicleID = -1;
    

        private static wDataEntryWindow _taskDataInputWindow = new wDataEntryWindow(new pTaskDataInput());
        public static wDataEntryWindow TaskDataInputWindow { get { if (_taskDataInputWindow.IsClosed) { _taskDataInputWindow = new wDataEntryWindow(new pTaskDataInput()); }; return _taskDataInputWindow; } }

        private static wDataEntryWindow _vehicleDataInputWindow = new wDataEntryWindow(new pVehicleDataInput());
        public static wDataEntryWindow VehicleDataInputWindow { get { if (_vehicleDataInputWindow.IsClosed) { _vehicleDataInputWindow = new wDataEntryWindow(new pVehicleDataInput()); }; return _vehicleDataInputWindow; } }

        public static wDataEntryWindow _logDataInputWindow = new wDataEntryWindow(new pLogDataInput());
        public static wDataEntryWindow LogDataInputWindow { get { if ( _logDataInputWindow.IsClosed) { _logDataInputWindow = new wDataEntryWindow(new pLogDataInput()); }; return _logDataInputWindow; } }

        public static wDataEntryWindow _scheduleDataInputWindow = new wDataEntryWindow(new pScheduleDataInput());
        public static wDataEntryWindow ScheduleDataInputWindow { get { if (_scheduleDataInputWindow.IsClosed) { _logDataInputWindow = new wDataEntryWindow(new pScheduleDataInput());  }; return _scheduleDataInputWindow; } }

    }
}
