using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using Dapper;

namespace VehicleMaintenanceLog.Classes
{
    internal class SqliteDataAccess
    {
        public static List<Vehicle> LoadVehicles()
        {

            try
            {
                return Query<Vehicle>("SELECT * FROM Vehicle", new DynamicParameters());
            }
            catch
            {
                return new List<Vehicle>();
            }
            
            
        }

        public static List<MaintenanceTask> LoadMaintenaceTasks(VehicleType type)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VT", type.ToString());

            return Query<MaintenanceTask>("select * from MaintenanceTask where VehicleType = @VT", parameters);
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

        public static MaintenanceLogItem GetMostRecentVehicleMaintenanceLog(int vehicleID, int taskID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<MaintenanceLogItem> logs = cnn.Query<MaintenanceLogItem>("SELECT * FROM MaintenanceLog WHERE (VehicleID = @vID AND TaskID = @tID) ORDER BY VehicleMileage DESC", new { vID = vehicleID, tID = taskID }).ToList();

                if(logs.Count > 0) return logs[0];
                else return null;
            }
        }

        public static VehicleType GetVehicleType(int vehicleID) => (VehicleType)Enum.Parse(typeof(VehicleType), Query<string>("SELECT VehicleType FROM Vehicle WHERE ID = " + vehicleID, new DynamicParameters())[0]);

        public static List<TaskSchedule> GetMaintenanceSchedules(VehicleType type, int vID)
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql =
                    @"SELECT * 
                    FROM MaintenanceSchedules ms LEFT JOIN MaintenanceTask mt 
                    ON ms.TaskID = mt.ID
                    WHERE (mt.VehicleType = @Type AND ms.AssignedVehicleID IS NULL)";

                List<TaskSchedule> defaultSchedulesOutput = new List<TaskSchedule>();
                try
                {
                    defaultSchedulesOutput = cnn.Query<TaskSchedule, MaintenanceTask, TaskSchedule>(sql,
                    (schedule, task) => schedule,
                    new { Type = type.ToString() },
                    splitOn: "ID").ToList();
                }
                catch { }


                if (vID != -1)
                {

                    string sql2 =
                        @"SELECT * 
                        FROM MaintenanceSchedules
                        WHERE AssignedVehicleID = @vID";

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@vID", vID);

                    List<TaskSchedule> vehicleSchedulesOutput = Query<TaskSchedule>(sql2, parameters);

                    foreach (TaskSchedule schedule in vehicleSchedulesOutput)
                    {
                        bool noDuplicate = true;
                        
                        for (int i = 0; i < defaultSchedulesOutput.Count; i++)
                        {
                            if (schedule.TaskID == defaultSchedulesOutput[i].TaskID)
                            {
                                defaultSchedulesOutput[i] = schedule;
                                noDuplicate = false;
                                break;
                            }
                        }

                        if(noDuplicate == true) defaultSchedulesOutput.Add(schedule);
                    }
                }

                return defaultSchedulesOutput;
            }
        }

        public static int GetNewestTaskID()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql = "SELECT ID FROM MaintenanceTask ORDER BY ID DESC";
                int output = cnn.QueryFirst<int>(sql, new {});
                return output;
            }
        }

        public static List<MaintenanceTask> GetTasks(VehicleType vehicleType)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@type", vehicleType.ToString());

            return Query<MaintenanceTask>("SELECT * FROM MaintenanceTask WHERE VehicleType = @type", parameters).ToList();
        }

        public static void SetVehicleMileage(int vehicleID, int mileage)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", vehicleID);
            parameters.Add("@Mileage", mileage);

            Execute("UPDATE Vehicle SET Mileage = @Mileage WHERE ID = @ID", parameters);
        }


        public static void CreateVehicle(Vehicle vehicle)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Type", vehicle.vehicleType.ToString());
            parameters.Add("@Name", vehicle.VehicleName);
            parameters.Add("@Mileage", vehicle.vehicleMileage);
            parameters.Add("@ManufactureDate", vehicle.manufactureDate.ToBinary());


            Execute("INSERT INTO Vehicle (VehicleType, MakeModel, Mileage, ManufactureDate) VALUES (@Type, @Name, @Mileage, @ManufactureDate)", parameters);
        }

        public static void EditVehicle(Vehicle vehicle)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@vID", vehicle.vehicleID);
            parameters.Add("@Type", vehicle.vehicleType.ToString());
            parameters.Add("@Name", vehicle.VehicleName);
            parameters.Add("@Mileage", vehicle.vehicleMileage);
            parameters.Add("@ManufactureDate", vehicle.manufactureDate.ToBinary());

            Execute("UPDATE Vehicle SET VehicleType = @Type, MakeModel = @Name, Mileage = @Mileage, ManufactureDate = @ManufactureDate WHERE ID = @vID", parameters);
        }

        public static void EditMaintenanceLog(int logID, MaintenanceLogItem log)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LID", logID);
            parameters.Add("@VID", log.VehicleID);
            parameters.Add("@TID", log.taskID);
            parameters.Add("@Mileage", log.VehicleMileage);
            parameters.Add("@Date", log.datecompleted.ToBinary());
            parameters.Add("@TmpFixMiles", log.tempFixMileage == -1 ? null : log.tempFixMileage);
            parameters.Add("@TmpFixTime", log.tempFixTime == -1 ? null : log.tempFixTime);
            parameters.Add("@Notes", log.LogNotes);

            Execute("UPDATE MaintenanceLog SET VehicleID = @VID, TaskID = @TID, VehicleMileage = @Mileage, DateCompleted = @Date, TempFixMileage = @TmpFixMiles, TempFixTime = @TmpFixTime, Notes = @Notes WHERE ID = @LID", parameters);

        }

        public static void EditTask(MaintenanceTask task)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tID", task.TaskID);
            parameters.Add("@name", task.TaskName);
            parameters.Add("@description", task.TaskDescription);

            Execute("UPDATE MaintenanceTask SET TaskName = @name, TaskDescription = @description WHERE ID = @tID", parameters);
        }

        public static void EditMaintenanceSchedule(TaskSchedule schedule)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@sID", schedule.ScheduleID);
            parameters.Add("@tID", schedule.TaskID);
            parameters.Add("vID", schedule.vehichleAssignedTo);
            if (schedule.timeIncrement != -1) parameters.Add("@TimeIncrement", schedule.TimeIncrement); else parameters.Add("@TimeIncrement", null);
            if (schedule.mileageIncrement != -1) parameters.Add("MileageIncrement", schedule.MileageIncrement); else parameters.Add("MileageIncrement", null);
            parameters.Add("@notes", schedule.ScheduleNotes);

            if (schedule.vehichleAssignedTo != -1 && GetValue<TaskSchedule>("MaintenanceSchedules", schedule.ScheduleID).vehichleAssignedTo == -1)
            {
                Execute("INSERT INTO MaintenanceSchedules (TaskID, AssignedVehicleID, TimeIncrement, DistanceIncrement, Notes) VALUES (@tID, @vID, @TimeIncrement, @MileageIncrement, @notes)", parameters);
            }
            else
            {
                if(schedule.vehichleAssignedTo != -1)
                {
                    Execute("UPDATE MaintenanceSchedules SET AssignedVehicleID = @vID, TimeIncrement = @TimeIncrement, DistanceIncrement = @MileageIncrement, Notes = @notes WHERE ID = @sID", parameters);
                }
                else
                {
                    Execute("UPDATE MaintenanceSchedules SET TimeIncrement = @TimeIncrement, DistanceIncrement = @MileageIncrement, Notes = @notes WHERE ID = @sID", parameters);
                }
            }

        }


        public static void CreateMaintenanceTask(MaintenanceTask task)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", task.TaskName);
            parameters.Add("@Type", task.vehicleType.ToString());

            Execute("INSERT INTO MaintenanceTask (TaskName, VehicleType) VALUES (@Name, @Type)", parameters);

            

        }

        public static void CreateTaskSchedule(TaskSchedule schedule)
        {

            DynamicParameters scheduleParameters = new DynamicParameters();
            scheduleParameters.Add("@tID", schedule.TaskID);
            scheduleParameters.Add("@vID", schedule.vehichleAssignedTo == -1 ? null : schedule.vehichleAssignedTo);
            scheduleParameters.Add("@TimeIncrement", schedule.timeIncrement == -1 ? null : schedule.TimeIncrement);
            scheduleParameters.Add("@MileageIncrement", schedule.mileageIncrement == -1 ? null : schedule.MileageIncrement);
            scheduleParameters.Add("@notes", schedule.ScheduleNotes == "" ? null : schedule.ScheduleNotes);

            Execute("INSERT INTO MaintenanceSchedules (TaskID, AssignedVehicleID, TimeIncrement, DistanceIncrement, Notes) VALUES (@tID, @vID, @TimeIncrement, @MileageIncrement, @notes)", scheduleParameters);
        }

        public static void CreateMaintenanceLog(MaintenanceLogItem log)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@VID", log.VehicleID);
            parameters.Add("@TID", log.taskID);
            parameters.Add("@Mileage", log.VehicleMileage);
            parameters.Add("@Date", log.datecompleted.ToBinary());
            parameters.Add("@TmpFixMiles", log.tempFixMileage == -1 ? null : log.tempFixMileage);
            parameters.Add("@TmpFixTime", log.tempFixTime == -1 ? null : log.tempFixTime);
            parameters.Add("@Notes", log.LogNotes);

            Execute("INSERT INTO MaintenanceLog (VehicleID, TaskID, VehicleMileage, DateCompleted, TempFixMileage, TempFixTime, Notes) VALUES (@VID, @TID, @Mileage, @Date, @TmpFixMiles, @TmpFixTime, @Notes)", parameters);
        }

        public static void DeleteMaintenanceLog(int logID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", logID);

            Execute("DELETE FROM MaintenanceLog WHERE ID = @ID", parameters);
        }

        public static void DeleteSchedule(int sID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@sID", sID);

            Execute("DELETE FROM MaintenanceSchedules WHERE ID = @sID", parameters);
        }

        public static void DeleteMaintenanceTask(MaintenanceTask task)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tID", task.TaskID);
            Execute("DELETE FROM MaintenanceTask WHERE ID = @tID", parameters);
        }

        public static void DeleteVehicle(int vID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@vID", vID);

            Execute("DELETE FROM MaintenanceSchedules WHERE AssignedVehicleID = @vID", parameters);
            Execute("DELETE FROM MaintenanceLog WHERE VehicleID = @vID", parameters);
            Execute("DELETE FROM Vehicle WHERE ID = @vID", parameters);
        }

        public static bool CheckMaintenanceTaskExists(int tID, int vID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tID", tID);
            parameters.Add("@vID", vID);

            if (vID == -1)
            {
                return (Query<int>("SELECT ID FROM MaintenanceSchedules WHERE TaskID = @tID AND AssignedVehicleID IS NULL", parameters).ToList().Count > 0);
            }
            else
            {
                return (Query<int>("SELECT ID FROM MaintenanceSchedules WHERE TaskID = @tID AND AssignedVehicleID = @vID", parameters).ToList().Count > 0);
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
}
