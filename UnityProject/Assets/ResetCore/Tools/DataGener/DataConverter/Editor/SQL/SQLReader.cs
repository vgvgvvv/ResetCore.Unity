#if MYSQL
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ResetCore.MySQL;
using ResetCore.Util;

namespace ResetCore.Data
{
    public class SQLReader : IDataReadable
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType dataType { get; set; }
        /// <summary>
        /// 数据库名
        /// </summary>
        public string database { get; private set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string id { get; private set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string pwd { get; private set; }
        /// <summary>
        /// 服务器ip
        /// </summary>
        public string host { get; private set; }
        /// <summary>
        /// 接口名
        /// </summary>
        public string port { get; private set; }

        /// <summary>
        /// 当前数据名
        /// </summary>
        public string currentDataTypeName { get; set; }

        /// <summary>
        /// 返回值域表
        /// </summary>
        public Dictionary<string, Type> fieldDict { get; private set; }

        /// <summary>
        /// 返回特性表
        /// </summary>
        public Dictionary<string, List<string>> attributeDict { get; private set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return MySQLManager.isOpen;
        }

        /// <summary>
        /// 返回地址
        /// </summary>
        public string filepath { get { return host; } }

        public SQLReader(string database, string tableName = "", string id = "root", string pwd = "123456", string host = "127.0.0.1", string port = "3306")
        {
            this.database = database;
            this.id = id;
            this.pwd = pwd;
            this.host = host;
            this.port = port;
            this.currentDataTypeName = tableName;

            MySQLManager.OpenSql(host, database, id, pwd, port);

            if (string.IsNullOrEmpty(currentDataTypeName))
            {
                string name = MySQLManager.GetAllTableName().GetRow(0).TryGet(0);
                currentDataTypeName = name;
            }

            fieldDict = new Dictionary<string, Type>();
            List<string> members = GetMemberNames();
            List<Type> types = GetMemberTypes();

            for (int i = 0; i < members.Count; i++)
            {
                fieldDict.Add(members[i], types[i]);
            }

            attributeDict = new Dictionary<string, List<string>>();
            var attibutes = GetAttributes();
            for (int i = 0; i < members.Count; i++)
            {
                attributeDict.Add(members[i], attibutes[i]);
            }
        }

        /// <summary>
        /// 获得注释
        /// </summary>
        /// <returns></returns>
        public List<string> GetComment()
        {
            return MySQLManager.GetComment(currentDataTypeName);
        }

        /// <summary>
        /// 获得列信息
        /// </summary>
        /// <param name="lineNum"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        public List<string> GetColume(int colNum, int startRow = 0, int endRow = -1)
        {
            if (!IsValid()) return null;

            List<string> res = new List<string>();
            List<string> allColData = MySQLManager.GetColumn(currentDataTypeName, colNum);
            allColData.CopyTo(res, startRow, endRow);
            return res;
        }

        /// <summary>
        /// 得到行信息
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        public List<string> GetRow(int rowNum, int startLine = 0, int endLine = -1)
        {
            if (!IsValid()) return null;

            List<string> res = new List<string>();
            List<string> allRowData = MySQLManager.GetRow(currentDataTypeName, rowNum);
            allRowData.CopyTo(res, startLine, endLine);
            return res;
        }

        /// <summary>
        /// 获取每列标题
        /// </summary>
        /// <returns></returns>
        public List<string> GetTitle()
        {
            if (!IsValid()) return null;

            return MySQLManager.GetTitle(currentDataTypeName);
        }

        /// <summary>
        /// 获取变量名
        /// </summary>
        /// <returns></returns>
        public List<string> GetMemberNames()
        {
            if (!IsValid()) return null;

            List<string> titles = GetTitle();
            List<string> memberNames = new List<string>();
            titles.ForEach((i, title) =>
            {
                if (!title.Contains("|"))
                {
                    memberNames.Add(title);
                }
                else
                {
                    string memberName = title.Split('|')[0];
                    memberNames.Add(memberName);
                }
            });
            return memberNames;
        }

        /// <summary>
        /// 获取变量名
        /// </summary>
        /// <returns></returns>
        public List<Type> GetMemberTypes()
        {
            if (!IsValid()) return null;

            List<string> titles = GetTitle();
            List<Type> result = new List<Type>();
            titles.ForEach((tit) =>
            {
                if (tit.Contains("|"))
                {
                    result.Add(tit.Split('|')[1].GetTypeByString());
                }
                else
                {
                    result.Add(typeof(string));
                }
            });
            return result;
        }

        public List<List<string>> GetAttributes()
        {
            List<string> title = GetTitle();
            List<List<string>> result = new List<List<string>>();

            title.ForEach((tit) =>
            {
                if (tit.Contains("|"))
                {
                    var strs = tit.Split('|');
                    if (strs.Length > 2)
                    {
                        result.Add(strs[2].GetValue<List<string>>());
                    }
                    else
                    {
                        result.Add(new List<string>());
                    }
                }
                else
                {
                    result.Add(new List<string>());
                }
            });
            return result;
        }

        /// <summary>
        /// 获得所有行对象
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetRowObjs(int start = 2)
        {
            if (!IsValid()) return null;
            List<Dictionary<string, object>> objList = new List<Dictionary<string, object>>();
            List<Dictionary<string, string>> rowsData = GetRows();
            //成员类型
            List<Type> typeList = GetMemberTypes();

            foreach (Dictionary<string, string> row in rowsData)
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                int index = 0;
                foreach (KeyValuePair<string, string> kvp in row)
                {
                    obj.Add(kvp.Key, kvp.Value.GetValue(typeList[index]));
                    index++;
                }
                objList.Add(obj);
            }

            return objList;
        }

        /// <summary>
        /// 获得所有行信息
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetRows()
        {
            if (!IsValid()) return null;
            var rows = MySQLManager.GetAllRow(currentDataTypeName);
            List<Dictionary<string, string>> res = new List<Dictionary<string, string>>();
            foreach (var row in rows)
            {
                var newRow = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> dict in row)
                {
                    newRow.Add(dict.Key.Split('|')[0], dict.Value);
                }
                res.Add(newRow);
            }
            return res;
        }

        /// <summary>
        /// 获得所有表名
        /// </summary>
        /// <returns></returns>
        public string[] GetSheetNames()
        {
            if (!IsValid()) return null;

            return MySQLManager.GetAllTableName().GetColume(0).ToArraySavely();
        }

    }
}
#endif