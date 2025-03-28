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


        /*void manualDataEntry()
        {
            //Add Vehicles
            SqliteDataAccess.CreateVehicle(new Vehicle("Ninja 400", VehicleType.Motorcycle, 1746, new DateTime(2023, 05, 17)));
            SqliteDataAccess.CreateVehicle(new Vehicle("Ford Fussion", VehicleType.Car, 122256, new DateTime(2016, 01, 01)));

            
            //Add Tasks
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Oil Change", VehicleType.Motorcycle, 7000, 12));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Check Chain Slack", VehicleType.Motorcycle, 500, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Clean and Lube Chain", VehicleType.Motorcycle, 400, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Fork Oil", VehicleType.Motorcycle, 2000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Check Spark Plugs", VehicleType.Motorcycle, 7000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Brake Fluid", VehicleType.Motorcycle, 15000, 24));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Air Filter", VehicleType.Motorcycle, 15000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Fuel Filter", VehicleType.Motorcycle, 15000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Coolant", VehicleType.Motorcycle, 22000, 36));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Tires", VehicleType.Motorcycle, -1, 60));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Bleed Brakes", VehicleType.Motorcycle, -1, 12));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Brake Hoses", VehicleType.Motorcycle, -1, 48));

            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Oil Change", VehicleType.Car, 5000, 12));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Replace Break Pads", VehicleType.Car, 20000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Replace Break Rotors", VehicleType.Car, 50000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Tires", VehicleType.Car, -1, 60));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Replace Cabin Air Filter", VehicleType.Car, 20000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Replace Engine Air Filter", VehicleType.Car, 30000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Check Engine Coolant", VehicleType.Car, 50000, 36));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Replace Spark Plugs", VehicleType.Car, 100000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Inspect Accessory Drive Belts", VehicleType.Car, 100000, -1));
            SqliteDataAccess.CreateMaintenanceTask(new MaintenanceTask("Change Automatic Transmission Fluid", VehicleType.Car, 150000, -1));



            //Add Logs

            //Ninja
            SqliteDataAccess.CreateMaintenanceLog(new MaintenanceLogItem(1, 1, 600, new DateTime(2023, 5, 21)));
            SqliteDataAccess.CreateMaintenanceLog(new MaintenanceLogItem(1, 2, 1300, new DateTime(2023, 10, 28)));

            //Fussion
            SqliteDataAccess.CreateMaintenanceLog(new MaintenanceLogItem(2, 3, 121800, new DateTime(2024, 8, 1))); //Oil Change
            SqliteDataAccess.CreateMaintenanceLog(new MaintenanceLogItem(2, 5, 123731, new DateTime(2024, 9, 27), "All 4 tires replaced and aligned")); //Tires
            SqliteDataAccess.CreateMaintenanceLog(new MaintenanceLogItem(2, 4, 125963, new DateTime(2024, 11, 17), "Rear pads and rotors replaced")); //Rotors/Pads
        }*/
    

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
