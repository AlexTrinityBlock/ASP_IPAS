using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using Newtonsoft.Json;
using ASP_IPAS.Models;

namespace ASP_IPAS.Models
{
    public class MySQLModel
    {
        public string connString;
        public MySqlConnection conn;

        public MySQLModel()
        {
            ConfigModel configModel = new ConfigModel();
            connString = "server=127.0.0.1;port=3306;user id=" + configModel.MySQLUser + ";password=" + configModel.MySQLPassword + ";database=mvcdb;charset=utf8;";
            conn = new MySqlConnection();
            conn.ConnectionString = connString;
            if (conn.State != ConnectionState.Open)
            {
                //如郭資料庫測試失敗
                if (!testDatabase())
                {
                    //重建資料庫
                    buildDatabase();
                }
            }

        }

        //測試資料庫
        public bool testDatabase()
        {
            return false;//測試

            //如果找不到目標資料表，則重建資料表
            try
            {
                getDataByUserName("Alice");
                getTaskData();
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
            return true;
        }

        //重建資料庫
        public void buildDatabase()
        {
            ConfigModel configModel = new ConfigModel();
            connString = "server=127.0.0.1;port=3306;user id=" + configModel.MySQLUser + ";password=" + configModel.MySQLPassword + ";charset=utf8;";
            conn = new MySqlConnection();
            conn.ConnectionString = connString;
            conn.Open();
            
            //重建資料庫
            string sql = @"CREATE DATABASE IF NOT EXISTS `mvcdb`;USE `mvcdb`;";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            
            //重建使用者資料表
            sql = @"
                CREATE TABLE IF NOT EXISTS `user` (
                  `id` int(11) NOT NULL AUTO_INCREMENT,
                  `name` varchar(50) DEFAULT NULL,
                  `password_hash` varchar(100) DEFAULT NULL,
                  PRIMARY KEY (`id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
            ";
            cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            sql = @"
                INSERT INTO `user` (`id`, `name`, `password_hash`) VALUES
                (1, 'Alice', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3');
            ";
            try
            {
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { }

            //重建任務排程資料庫
            sql = @"
                CREATE TABLE IF NOT EXISTS `tasklist` (
                  `id` int(11) NOT NULL AUTO_INCREMENT,
                  `taskName` varchar(50) NOT NULL DEFAULT '',
                  `startTime` varchar(50) NOT NULL DEFAULT '',
                  `endTime` varchar(50) NOT NULL DEFAULT '',
                  `needNumber` varchar(50) NOT NULL DEFAULT '',
                  `finishedNumber` varchar(50) NOT NULL DEFAULT '',
                  PRIMARY KEY (`id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
            ";
            //
            cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        //以使用者名稱取得使用者資料
        public UserDataModel getDataByUserName(string userName)
        {
            conn.Open();
            string sql = @"SELECT * FROM `user` WHERE `name` = @userName";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.Add("@userName", MySqlDbType.VarChar);
            cmd.Parameters["@userName"].Value = userName;

            MySqlDataReader sdr = cmd.ExecuteReader();

            UserDataModel userDataModel = new UserDataModel();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    userDataModel.id = sdr["id"].ToString();
                    userDataModel.name = sdr["name"].ToString();
                    userDataModel.password_hash= sdr["password_hash"].ToString();
                }
            }

            return userDataModel;
        }

        //取得工作任務資料
        public string getTaskData()
        {
            conn.Open();
            string sql = @"SELECT * FROM `tasklist` ";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            MySqlDataReader sdr = cmd.ExecuteReader();

            List<TaskData> taskDataList = new List<TaskData>();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    TaskData taskData = new TaskData();
                    taskData.taskName = sdr["taskName"].ToString();
                    taskData.startTime = sdr["startTime"].ToString();
                    taskData.endTime = sdr["endTime"].ToString();
                    taskData.needNumber = sdr["needNumber"].ToString();
                    taskData.finishedNumber = sdr["finishedNumber"].ToString();
                    taskDataList.Add(taskData);
                }
            }

            conn.Close();

            string result = "";
            result += "[";
            foreach (TaskData taskData in taskDataList)
            {
                result += "{";
                result += "\"taskName\":\"" + taskData.taskName + "\",";
                result += "\"startTime\":\"" + taskData.startTime + "\",";
                result += "\"endTime\":\"" + taskData.endTime + "\",";
                result += "\"needNumber\":\"" + taskData.needNumber + "\",";
                result += "\"finishedNumber\":\"" + taskData.finishedNumber + "\"";
                result += "},";
            }

            result = result.Remove(result.Length - 1);

            result += "]";

            return result;
        }

        //設置工作任務資料
        public void setTaskData(TaskData taskDataObj)
        {
            conn.Open();
            string sql = @"INSERT INTO `tasklist` (`taskName`,`startTime`,`endTime`,`needNumber`,`finishedNumber`) VALUES (@taskName,@startTime,@endTime,@needNumber,@finishedNumber)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.Add("@taskName", MySqlDbType.VarChar);
            cmd.Parameters["@taskName"].Value = taskDataObj.taskName;

            cmd.Parameters.Add("@startTime", MySqlDbType.VarChar);
            cmd.Parameters["@startTime"].Value = taskDataObj.startTime;

            cmd.Parameters.Add("@endTime", MySqlDbType.VarChar);
            cmd.Parameters["@endTime"].Value = taskDataObj.endTime;

            cmd.Parameters.Add("@needNumber", MySqlDbType.VarChar);
            cmd.Parameters["@needNumber"].Value = taskDataObj.needNumber;

            cmd.Parameters.Add("@finishedNumber", MySqlDbType.VarChar);
            cmd.Parameters["@finishedNumber"].Value = taskDataObj.finishedNumber;

            cmd.ExecuteNonQuery();
            conn.Close();

        }
    }
}