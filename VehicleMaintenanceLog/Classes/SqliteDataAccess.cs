using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.Classes
{
    internal class SqliteDataAccess
    {

        public static List<T> GetAllOfType<T>()
        {

            try { return Query<T>("SELECT * FROM " + new SqlItemInfo<T>().tableName, new DynamicParameters()); }
            catch { return new List<T>(); }
        }

        public static T GetItem<T>(int id)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    return cnn.QueryFirst<T>("SELECT * FROM " + new SqlItemInfo<T>().tableName + " WHERE ID = @id", parameters);
                }
                catch
                {
                    return default(T);
                }
            }
        }

        public static int GetNewestItemID<T>()
        {

            string table = "";

            if (typeof(T).IsAssignableTo(typeof(Vehicle))) table = "Vehicle";
            else if (typeof(T).IsAssignableTo(typeof(MaintenanceProfile))) table = "MaintenanceProfile";
            else if (typeof(T).IsAssignableTo(typeof(MaintenanceLogItem))) table = "MaintenanceLog";
            else if (typeof(T).IsAssignableFrom(typeof(MaintenanceTask))) table = "MaintenanceTask";
            else throw new Exception("Type does not have a associated table");

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql = "SELECT ID FROM " + table + " ORDER BY ID DESC";
                int output = cnn.QueryFirst<int>(sql, new { });
                return output;
            }
        }


        public static List<MaintenanceTask> GetMaintenanceTasks(int _mpID)
        {


            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql =
                    @"SELECT * 
                    FROM TaskSchedule ms LEFT JOIN MaintenanceTask mt 
                    ON ms.TaskID = mt.ID
                    WHERE ms.MaintenanceProfileID = @mpID";

                List<MaintenanceTask> maintenanceTasks = new List<MaintenanceTask>();
                try
                {
                    maintenanceTasks = cnn.Query<TaskSchedule, MaintenanceTask, MaintenanceTask>(sql,
                    (schedule, task) => task,
                    new { mpID = _mpID },
                    splitOn: "ID").ToList();
                }
                catch { }

                return maintenanceTasks;
            }

        }

        public static List<MaintenanceLogItem> GetEntireVehicleMaintenanceLog(int vehicleID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", vehicleID);

            try
            {
                return Query<MaintenanceLogItem>("select * from MaintenanceLog where VehicleID = @ID order by VehicleMileage DESC", parameters);
            }
            catch
            {
                return new List<MaintenanceLogItem>();
            }

        }

        public static List<MaintenanceTask> GetMaintenanceTasks(VehicleType vehicleType)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VehicleType", vehicleType.ToString());

            try
            {
                return Query<MaintenanceTask>("Select * from MaintenanceTask where VehicleType = @VehicleType ORDER BY TaskName DESC", parameters);
            }
            catch { return new List<MaintenanceTask>(); }
        }

        public static List<int> GetTaskIDsInProfile(int profileID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ProfileID", profileID);

            return Query<int>("SELECT TaskID FROM TaskSchedule WHERE MaintenanceProfileID = @ProfileID", parameters);
        }

        public static List<TaskSchedule> GetSchedulesInProfile(int profileID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ProfileID", profileID);

            return Query<TaskSchedule>("SELECT * FROM TaskSchedule WHERE MaintenanceProfileID = @ProfileID", parameters);
        }

        public static MaintenanceLogItem GetMostRecentVehicleMaintenanceLog(int vehicleID, int taskID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<MaintenanceLogItem> logs = cnn.Query<MaintenanceLogItem>("SELECT * FROM MaintenanceLog WHERE (VehicleID = @vID AND TaskID = @tID) ORDER BY VehicleMileage DESC", new { vID = vehicleID, tID = taskID }).ToList();

                if (logs.Count > 0) return logs[0];
                else return null;
            }
        }
        public static List<TaskSchedule> GetMaintenanceSchedules(Vehicle v)
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql =
                    @"SELECT * 
                    FROM TaskSchedule ms LEFT JOIN MaintenanceTask mt 
                    ON ms.TaskID = mt.ID
                    WHERE ms.MaintenanceProfileID = @mpID";

                List<TaskSchedule> defaultSchedulesOutput = new List<TaskSchedule>();
                try
                {
                    defaultSchedulesOutput = cnn.Query<TaskSchedule, MaintenanceTask, TaskSchedule>(sql,
                    (schedule, task) => schedule,
                    new { mpID = v.maintenanceProfileID },
                    splitOn: "ID").ToList();
                }
                catch { }

                return defaultSchedulesOutput;
            }
        }


        public static void CreateItem<T>(T item)
        {
            SqlItemInfo<T> info = new SqlItemInfo<T>(item);

            Execute("INSERT INTO " + info.tableName + " " + info.createStr, info.parameters);
        }

        public static void EditItem<T>(T item)
        {         
            SqlItemInfo<T> info = new SqlItemInfo<T>(item);

            Execute("UPDATE "  + info.tableName + " SET " + info.editStr + " WHERE ID = @id", info.parameters);
        }

        public static void DeleteItem<T>(int itemID)
        {
            SqlItemInfo<T> info = new SqlItemInfo<T>();

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                if(typeof(T) == typeof(Vehicle))
                {
                    cnn.Execute("DELETE FROM TaskSchedule WHERE AssignedVehicleID = @id", new { id = itemID });
                    cnn.Execute("DELETE FROM MaintenanceLog WHERE VehicleID = @id", new { id = itemID });
                }
                else if(typeof(T) == typeof(MaintenanceProfile))
                {
                    cnn.Execute("DELETE FROM TaskSchedule WHERE MaintenanceProfileID = @id", new { id = itemID });
                    cnn.Execute("UPDATE Vehicle SET MaintenanceProfileID = 0 WHERE MaintenanceProfileID = @id", new { id = itemID });
                }

                cnn.Execute("DELETE FROM " + info.tableName + " WHERE ID = @id", new { id = itemID });
            }
        }

        public static bool CheckMaintenanceTaskExists(int tID, int vID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tID", tID);
            parameters.Add("@vID", vID);

            if (vID == -1)
            {
                return (Query<int>("SELECT ID FROM TaskSchedule WHERE TaskID = @tID AND AssignedVehicleID IS NULL", parameters).ToList().Count > 0);
            }
            else
            {
                return (Query<int>("SELECT ID FROM TaskSchedule WHERE TaskID = @tID AND AssignedVehicleID = @vID", parameters).ToList().Count > 0);
            }

        }


        private static void Execute(string command, DynamicParameters param)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute(command, param);
            }
        }


        private static List<T> Query<T>(string query, DynamicParameters param)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                var output = cnn.Query<T>(query, param);
                return output.ToList();

            }
        }


        public static T GetValue<T>(string tableName, int itemID, string column = "*")
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string queryTxt = "SELECT " + column + " FROM " + tableName + " WHERE ID = @ItemID";
                T output = cnn.QuerySingle<T>(queryTxt, new { ItemID = itemID });
                return output;
            }
        }


        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }


    class SqlItemInfo<T>
    {
        public readonly string tableName;
        public readonly DynamicParameters parameters;
        public readonly string createStr;
        public readonly string editStr;
            
        public SqlItemInfo()
        {

            if (typeof(T) == typeof(Vehicle))
            {
                tableName = "Vehicle";
                createStr = "(VehicleType, MakeModel, Mileage, ManufactureDate, MaintenanceProfileID) VALUES (@Type, @Name, @Mileage, @ManufactureDate, @MaintenanceProfileID)";
                editStr = "VehicleType = @Type, MakeModel = @Name, Mileage = @Mileage, ManufactureDate = @ManufactureDate, MaintenanceProfileID = @MaintenanceProfileID";
            }
            else if (typeof(T) == typeof(TaskSchedule))
            {
                tableName = "TaskSchedule";
                createStr = "(TaskID, MaintenanceProfileID, TimeIncrement, DistanceIncrement, Notes) VALUES (@tID, @pID, @tIncrement, @dIncrement, @Notes)";
                editStr = "TaskID = @tID, MaintenanceProfileID = @pID, TimeIncrement = @tIncrement, DistanceIncrement = @dIncrement, Notes = @Notes";
            }
            else if (typeof(T) == typeof(MaintenanceLogItem))
            {
                tableName = "MaintenanceLog";
                createStr = "(VehicleID, TaskID, VehicleMileage, DateCompleted, TempFixMileage, TempFixTime, Notes) VALUES (@vID, @tID, @Mileage, @Date, @TmpFixMiles, @TmpFixTime, @Notes)";
                editStr = "VehicleID = @vID, TaskID = @tID, VehicleMileage = @Mileage, DateCompleted = @Date, TempFixMileage = @TmpFixMiles, TempFixTime = @TmpFixTime, Notes = @Notes";
            }
            else if (typeof(T) == typeof(MaintenanceTask))
            {
                tableName = "MaintenanceTask";
                createStr = "(TaskName, VehicleType, TaskDescription) VALUES (@name, @vType, @description)";
                editStr = "TaskName = @name, VehicleType = @vType, TaskDescription = @description";
            }
            else if (typeof(T) == typeof(MaintenanceProfile))
            {
                tableName = "MaintenanceProfile";
                createStr = "(ProfileName) VALUES (@Name)";
                editStr = "ProfileName = @name";
            }
            else throw new Exception("Type Not Accepted");
        }

        public SqlItemInfo(T item) : this() 
        {

            if (item is Vehicle)
            {
                Vehicle v = item as Vehicle;
                parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    { "@id", v.id },
                    { "@Type", v.type.ToString() },
                    { "@Name", v.name },
                    { "@Mileage", v.mileage },
                    { "@ManufactureDate", v.manufactureDate.ToBinary() },
                    { "@MaintenanceProfileID", v.maintenanceProfileID }
                });
            }
            else if (item is TaskSchedule)
            {
                TaskSchedule s = item as TaskSchedule;
                parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    { "@id", s.id },
                    { "@tID", s.taskID },
                    { "@pID", s.maintenanceProfileID },
                    { "@tIncrement", s.timeIncrement },
                    { "@dIncrement", s.mileageIncrement },
                    { "@Notes", s.scheduleNotes }
                });
            }
            else if (item is MaintenanceLogItem)
            {
                MaintenanceLogItem l = item as MaintenanceLogItem;
                parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    { "@id", l.LogID },
                    { "@vID", l.VehicleID },
                    { "@tID", l.taskID },
                    { "@Mileage", l.VehicleMileage },
                    { "@Date", l.datecompleted.ToBinary() },
                    { "@TmpFixMiles", l.tempFixMileage == -1 ? null : l.tempFixMileage },
                    { "@TmpFixTime", l.tempFixTime == -1 ? null : l.tempFixTime },
                    { "@Notes", l.LogNotes }
                });
            }
            else if (item is MaintenanceTask)
            {
                MaintenanceTask t = item as MaintenanceTask;
                parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    { "@id", t.id },
                    { "@name", t.name },
                    { "@vType", t.vehicleType },
                    { "@description", t.description }
                });
            }
            else if (item is MaintenanceProfile)
            {
                MaintenanceProfile p = item as MaintenanceProfile;
                parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    { "id", p.id },
                    { "name", p.name },
                    { "typeConstraint", p.vehicleTypeConstraint.ToString() }
                });
            }

        }

    }


}
