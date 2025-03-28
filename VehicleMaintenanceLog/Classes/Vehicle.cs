using System;

namespace VehicleMaintenanceLog.Classes
{
    public class Vehicle
    {

        public int vehicleID;
        public string VehicleName { get; set; } = "";
        public VehicleType vehicleType;
        public int vehicleMileage;
        public DateTime manufactureDate;


        public Vehicle(string _name, VehicleType _type, int _mileage, DateTime _manufactureDate, int id = -1)
        {
            vehicleID = id;
            VehicleName = _name;
            vehicleType = _type;
            vehicleMileage = _mileage;
            manufactureDate = _manufactureDate;
        }


        public Vehicle(Int64 ID, String VehicleType, String MakeModel, Int64 Mileage, Int64 manufactureDate) //SQLAccess Constructor
        {
            vehicleID = (int)ID;
            Enum.TryParse(VehicleType, out vehicleType);
            VehicleName = MakeModel;
            vehicleMileage = (int)Mileage;
            this.manufactureDate = DateTime.FromBinary(manufactureDate);
        }



    }


    public enum VehicleType
    {
        Car,
        Motorcycle
    }
}
