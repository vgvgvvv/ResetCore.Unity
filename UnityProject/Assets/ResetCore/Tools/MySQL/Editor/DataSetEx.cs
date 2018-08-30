using UnityEngine;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using ResetCore.Util;

namespace ResetCore.MySQL
{
    public static class DataSetEx
    {
        /// <summary>
        /// 获取行信息
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public static List<string> GetRow(this DataSet dataSet, int index, int tableIndex = 0)
        {
            List<string> result = new List<string>();
            foreach (DataColumn col in dataSet.Tables[tableIndex].Columns)
            {
                result.Add(dataSet.Tables[tableIndex].Rows[index][col.ColumnName].ConverToString());
            }
            return result;
        }

        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public static List<string> GetColume(this DataSet dataSet, int index, int tableIndex = 0)
        {
            List<string> result = new List<string>();
            foreach (DataRow row in dataSet.Tables[tableIndex].Rows)
            {
                result.Add(row[index].ConverToString());
            }
            return result;
        }

        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="index"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public static List<string> GetColume(this DataSet dataSet, string colName, int tableIndex = 0)
        {
            List<string> result = new List<string>();
            foreach (DataRow row in dataSet.Tables[tableIndex].Rows)
            {
                result.Add(row[colName].ConverToString());
            }
            return result;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <param name="colName"></param>
        /// <param name="row"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataSet dataSet, string colName, int row, int tableIndex = 0)
        {
            return (T)dataSet.Tables[tableIndex].Rows[row][colName];
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataSet dataSet, int col, int row, int tableIndex = 0)
        {
            return (T)dataSet.Tables[tableIndex].Rows[row][col];
        }

    }
}

