using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ResetCore.Data
{
    public enum DataType
    {
        Normal,
        Pref
    }

    public interface IDataReadable
    {
        /// <summary>
        /// 数据类型Normal或者Pref
        /// </summary>
        DataType dataType { get; }
        /// <summary>
        /// 当前的数据类型名称
        /// </summary>
        string currentDataTypeName { get; set; }
        /// <summary>
        /// 数据的域信息
        /// </summary>
        Dictionary<string, Type> fieldDict { get; }
        /// <summary>
        /// 数据的属性信息
        /// </summary>
        Dictionary<string, List<string>> attributeDict { get; }
        /// <summary>
        /// 文件路径或者数据库url
        /// </summary>
        string filepath { get; }
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        bool IsValid();
        /// <summary>
        /// 获取所有的表头
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        List<string> GetTitle();
        /// <summary>
        /// 获取所有成员名
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        List<string> GetMemberNames();
        /// <summary>
        /// 获取所有的类型名
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        List<Type> GetMemberTypes();
        /// <summary>
        /// 获取该域的特性
        /// </summary>
        /// <returns></returns>
        List<List<string>> GetAttributes();
        /// <summary>
        /// 获取注释
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        List<string> GetComment();
        /// <summary>
        /// 获取表名列表
        /// </summary>
        /// <returns></returns>
        string[] GetSheetNames();
        /// <summary>
        /// 获取所有行信息
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        List<Dictionary<string, string>> GetRows();
        /// <summary>
        /// 获取行信息
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        List<string> GetRow(int rowNum, int startLine = 0, int endLine = -1);
        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="lineNum"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <returns></returns>
        List<string> GetColume(int lineNum, int startRow = 0, int endRow = -1);
        /// <summary>
        /// 获取行单位
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetRowObjs(int start = 2);
    }

}
