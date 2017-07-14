using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ResetCore.Util
{
    public class LanguageData : Attribute
    {
        public LanguageData() { }
        public static void HandleProperty(object obj, PropertyInfo info)
        {
            //if (info.PropertyType != typeof(string))
            //    return;

            //int id = (int)info.GetValue(obj, null);
            //info.SetValue(obj, LanguageManager.GetString(id), null);
        }
#if UNITY_EDITOR
        public static string HandleExportXmlValue(string value)
        {
            //string v = value.Default2UTF8();
            //MySQLManager.OpenSql("192.168.0.31", "app_language", "root", "@ssjj@");
            //string selectsql = "select * from `words` where `word` = '" + v + "'";
            //var dataset = MySQLManager.ExecuteQuery(selectsql);

            //if (dataset.Tables[0].Rows.Count == 0)
            //{
            //    string sql = "insert ignore into words(word, add_time) values('" + v + "', now())";
            //    MySQLManager.ExecuteQuery(sql);
            //}

            //dataset = MySQLManager.ExecuteQuery(selectsql);
            //var id = dataset.GetValue<UInt32>("id", 0);

            //return id.ToString();
            return value;
        }
#endif
    }
    public class DataAttributes
    {
        public static Dictionary<string, Type> attributes = new Dictionary<string, Type>()
        {
            { "l", typeof(LanguageData)}
        };
    }

}
