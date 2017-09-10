using UnityEngine;
using System.Collections;
using ResetCore.Util;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Collections.Generic;
using System;
using System.Data;

namespace ResetCore.MySQL
{
    public class MySQLManager
    {
        /// <summary>
        /// 是否被打开
        /// </summary>
        public static bool isOpen
        {
            get
            {
                return current != null;
            }
        }

        private static MySqlConnection current { get; set; }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="host">主机IP</param>
        /// <param name="database">数据库名</param>
        /// <param name="id">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="port">端口（默认为3306）</param>
        public static void OpenSql(string host, string database = "", string id = "root", string pwd = "", string port = "3306")
        {
            if (current != null)
            {
                current.Close();
            }
            try
            {
                if (!string.IsNullOrEmpty(database))
                {
                    string connectionString =
                       string.Format("Server = {0};port={4};Database = {1}; User ID = {2}; Password = {3};", host, database, id, pwd, port);
                    current = new MySqlConnection(connectionString);
                    current.Open();
                }
                else
                {
                    string connectionString =
                       string.Format("Server = {0};port={3}; User ID = {1}; Password = {2};", host, id, pwd, port);
                    current = new MySqlConnection(connectionString);
                    current.Open();
                    OpenSql(host, GetAllDatabaseName().GetColume(0)[0], id, pwd, port);
                    
                }
              
            }
            catch (Exception e)
            {
                Close();
                Debug.LogException(e);
                throw new Exception("服务器连接失败，请重新检查是否打开MySql服务。" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public static void Close()
        {

            if (current != null)
            {
                current.Close();
                current.Dispose();
                current = null;
            }

        }

        /// <summary>
        /// 执行SQL操作
        /// </summary>
        /// <param name="sqlString">需要执行的字符串</param>
        /// <returns>返回相应的DataSet</returns>
        public static DataSet ExecuteQuery(string sqlString)
        {
            if (current.State == ConnectionState.Open)
            {
                DataSet ds = new DataSet();
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(sqlString, current);
                    da.Fill(ds);

                }
                catch (Exception ee)
                {
                    Close();
                    Debug.unityLogger.LogException(ee);
                    throw new Exception("SQL:" + sqlString + "/n" + ee.Message.ToString());
                }
                return ds;
            }
            return null;
        }

        /// <summary>
        /// 获取注释
        /// </summary>
        /// <returns></returns>
        public static List<string> GetComment(string tableName)
        {
            DataSet fullInfo = GetFullTableInfo(tableName);
            return fullInfo.GetColume("Comment");
        }

        /// <summary>
        /// 获取域信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<string> GetTitle(string tableName)
        {
            DataSet fullInfo = GetFullTableInfo(tableName);
            return fullInfo.GetColume("Field");
        }


        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public static List<string> GetColumn(string tableName, int lineNumber)
        {
            DataSet fullInfo = GetDataSet(tableName);
            return fullInfo.GetColume(lineNumber);
        }

        /// <summary>
        /// 获取行信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public static List<string> GetRow(string tableName, int rowNumber)
        {
            DataSet fullInfo = GetDataSet(tableName);
            return fullInfo.GetRow(rowNumber);
        }

        /// <summary>
        /// 获取所有的行信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> GetAllRow(string tableName)
        {
            DataSet fullInfo = GetDataSet(tableName);
            List<Dictionary<string, string>> res = new List<Dictionary<string, string>>();
            
            foreach(DataRow row in fullInfo.Tables[0].Rows)
            {
                Dictionary<string, string> rowData = new Dictionary<string, string>();
                foreach (DataColumn col in fullInfo.Tables[0].Columns)
                {
                    rowData.Add(col.ColumnName, row[col.ColumnName].ConverToString());
                }
                res.Add(rowData);
            }
            return res;
        }

        /// <summary>
        /// 获取完整的表信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet GetFullTableInfo(string tableName)
        {
            return ExecuteQuery("show full columns from " + tableName);
        }

        /// <summary>
        /// 读取整张数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string tableName)
        {
            return ExecuteQuery("SELECT * FROM `" + tableName + "` WHERE 1");
        }

        /// <summary>
        /// 查找所有表名
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllTableName()
        {
            return ExecuteQuery("show tables");
        }

        /// <summary>
        /// 显示所有数据库名
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllDatabaseName()
        {
            return ExecuteQuery("show databases");
        }

        /// <summary>
        /// 设置当前数据库
        /// </summary>
        /// <param name="databaseName"></param>
        public static void SetDatabase(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName)) return;
            ExecuteQuery("use " + databaseName);
        }

    }
}

