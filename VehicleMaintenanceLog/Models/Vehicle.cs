using System;

namespace VehicleMaintenanceLog.Models
{
    public class Vehicle
    {

        public int id;
        public string name = "";
        public VehicleType type;
        public int mileage;
        public DateTime manufactureDate;

        public Vehicle(int _id = -1, string _name = "", VehicleType _type = VehicleType.Car, int _mileage = 0, DateTime _manufactureDate = default)
        {
            id = _id;
            name = _name;
            type = _type;
            mileage = _mileage;
            manufactureDate = _manufactureDate == default ? DateTime.Now : _manufactureDate;
        }


        public Vehicle(Int64 ID, String VehicleType, String MakeModel, Int64 Mileage, Int64 manufactureDate) //SQLAccess Constructor
        {
            id = (int)ID;
            Enum.TryParse(VehicleType, out type);
            name = MakeModel;
            mileage = (int)Mileage;
            this.manufactureDate = DateTime.FromBinary(manufactureDate);
        }

    }


    public enum VehicleType
    {
        Car,
        Motorcycle
    }
}
