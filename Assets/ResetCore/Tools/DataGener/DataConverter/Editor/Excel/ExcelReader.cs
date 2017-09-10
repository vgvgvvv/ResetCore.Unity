using UnityEngine;
using System.Collections;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using ResetCore.Util;
using ResetCore.Data;

namespace ResetCore.Excel
{
    public class ExcelReader : IDataReadable
    {
        public DataType dataType { get; set; }
        private readonly IWorkbook workbook = null;
        private ISheet sheet { get; set; }
        public string filepath { get; private set; }
        public string currentDataTypeName { get; set; }

        public Dictionary<string, Type> fieldDict { get; private set; }

        public Dictionary<string, List<string>> attributeDict { get; private set; }

        public ExcelReader(string path, string sheetName = "", DataType type = DataType.Normal)
        {
            try
            {
                this.filepath = path;
                this.currentDataTypeName = sheetName;
                this.dataType = type;

                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    string extension = Path.GetExtension(path);
                    //根据不同的后缀进行解析
                    if (extension == ".xls")
                        workbook = new HSSFWorkbook(fileStream);
                    else if (extension == ".xlsx")
                    {
#if UNITY_MAC
                    throw new Exception("xlsx is not supported on OSX.");
#else
                        workbook = new XSSFWorkbook(fileStream);
#endif
                    }
                    else
                    {
                        throw new Exception("Wrong file.");
                    }

                    if (!string.IsNullOrEmpty(currentDataTypeName))
                    {
                        sheet = workbook.GetSheet(currentDataTypeName);
                    }
                    else
                    {
                        if (workbook.GetSheetAt(0) != null)
                        {
                            sheet = workbook.GetSheetAt(0);
                            currentDataTypeName = sheet.SheetName;
                        }
                        else
                        {
                            Debug.unityLogger.LogError("获取Sheet", "该Excel不存在Sheet");
                        }

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
            }
            catch (Exception e)
            {
                Debug.unityLogger.LogException(e);
            }
        }

        /// <summary>
        /// Excel是否有效
        /// </summary>
        public bool IsValid()
        {
            if (this.workbook != null && this.sheet != null)
                return true;

            return false;
        }


        /// <summary>
        /// 获得完整的表头
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<string> GetTitle()
        {
            List<string> result = new List<string>();

            if (dataType == DataType.Normal)
            {
                #region Normal
                return GetRow(0);
                #endregion
            }
            else if (dataType == DataType.Pref)
            {
                #region Pref
                return GetColume(0);
                #endregion
            }
            else
            {
                return result;
            }

        }

        /// <summary>
        /// 获取标题
        /// </summary>
        public List<string> GetMemberNames()
        {
            List<string> title = GetTitle();
            List<string> result = new List<string>();
            title.ForEach((tit) =>
            {
                if (tit.Contains("|"))
                {
                    result.Add(tit.Split('|')[0]);
                }
                else
                {
                    result.Add(tit);
                }
            });
            return result;
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        public List<Type> GetMemberTypes()
        {
            List<string> title = GetTitle();
            List<Type> result = new List<Type>();

            title.ForEach((tit) =>
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
        /// 获取注释
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<string> GetComment()
        {
            List<string> result = new List<string>();
            if (dataType == DataType.Normal)
            {
                #region Normal
                return GetRow(1);
                #endregion
            }
            else if (dataType == DataType.Pref)
            {
                #region Pref
                return GetColume(1);
                #endregion
            }
            else
            {
                return result;
            }

        }

        #region Normal

        /// <summary>
        /// 得到表头
        /// </summary>
        /// <param name="cellnum"></param>
        /// <returns></returns>
        string GetHeaderColumnName(int cellnum)
        {
            ICell headerCell = sheet.GetRow(0).GetCell(cellnum);
            if (headerCell != null)
                return headerCell.StringCellValue;
            return string.Empty;
        }

        /// <summary>
        /// Deserialize all the cell of the given sheet.
        /// 
        /// NOTE:
        ///     The first row of a sheet is header column which is not the actual value 
        ///     so it skips when it deserializes.
        ///     第一排和第二排为标题与变量类型，所以在解析时跳过
        /// </summary>
        public List<T> Deserialize<T>(int start = 2)
        {
            var t = typeof(T);
            PropertyInfo[] p = t.GetProperties();

            var result = new List<T>();

            int current = 0;
            foreach (IRow row in sheet)
            {
                if (current < start)
                {
                    current++; // skip header column.
                    continue;
                }

                var item = (T)Activator.CreateInstance(t);
                for (var i = 0; i < p.Length; i++)
                {
                    ICell cell = row.GetCell(i);

                    var property = p[i];
                    if (property.CanWrite)
                    {
                        try
                        {
                            var value = cell.StringCellValue.GetValue<T>();
                            property.SetValue(item, value, null);
                        }
                        catch (Exception e)
                        {
                            string pos = string.Format("Row[{0}], Cell[{1}]", (current + 1).ToString(), GetHeaderColumnName(i));
                            Debug.LogError(string.Format("Excel File {0} Deserialize Exception: {1} at {2}", this.filepath, e.Message, pos));
                        }
                    }
                }

                result.Add(item);

                current++;
            }

            return result;
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        public string[] GetSheetNames()
        {
            List<string> sheetList = new List<string>();
            if (this.workbook != null)
            {
                int numSheets = this.workbook.NumberOfSheets;
                for (int i = 0; i < numSheets; i++)
                {
                    string sheetName = this.workbook.GetSheetName(i).Trim();
                    if (string.IsNullOrEmpty(sheetName)) break;
                    if (string.IsNullOrEmpty(sheetName))
                    {
                        Debug.unityLogger.LogError("ReadExcelError", "SheetName " + i + " is Null or Empty");
                    }
                    sheetList.Add(sheetName);
                }
            }
            else
            {
                Debug.LogError("Workbook is null. Did you forget to import excel file first?");
            }
            return (sheetList.Count > 0) ? sheetList.ToArray() : null;
        }


        /// <summary>
        /// 获取每一行数据
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetRows()
        {
            int start = 2;
            List<string> clomnNameList = GetMemberNames();
            int length = clomnNameList.Count;
            List<string> tagName = new List<string>();
            for (int i = 0; i < length; i++)
            {
                tagName.Add(clomnNameList[i]);
            }

            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

            int current = 0;
            foreach (IRow row in sheet)
            {
                if (current < start)
                {
                    current++;
                    continue;
                }

                Dictionary<string, string> rowDict = new Dictionary<string, string>();
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        row.GetCell(i).SetCellType(CellType.String);
                        string cellStr = row.GetCell(i).StringCellValue.Trim();
                        if (!string.IsNullOrEmpty(cellStr))
                        {
                            rowDict.Add(tagName[i], row.GetCell(i).StringCellValue);
                        }
                        else
                        {
                            Debug.unityLogger.LogError("ReadExcelError", string.Format("Value in Posistion row[{0}] line[{1}] is Error", current, i));
                            return rows;
                        }

                    }
                    catch (System.Exception ex)
                    {
                        Debug.unityLogger.LogException(ex);
                        Debug.unityLogger.LogError("ReadExcelError", string.Format("Value in Posistion row[{0}] line[{1}] is Error", current, i));
                    }

                }
                rows.Add(rowDict);
                current++;
            }

            return rows;
        }

        /// <summary>
        /// 得到一行,如果未输入结束列则遇到空结束
        /// </summary>
        /// <param name="rowNum"></param>
        /// <returns></returns>
        public List<string> GetRow(int rowNum, int startLine = 0, int endLine = -1)
        {
            List<string> result = new List<string>();
            IRow typeStr = sheet.GetRow(rowNum);
            if (typeStr != null)
            {
                int lastLineNum = typeStr.LastCellNum;
                if (endLine > 0)
                    lastLineNum = endLine;

                for (int i = startLine; i < lastLineNum; i++)
                {
                    try
                    {
                        typeStr.Cells[i].SetCellType(CellType.String);
                        if (!string.IsNullOrEmpty(typeStr.Cells[i].StringCellValue))
                        {
                            result.Add(typeStr.Cells[i].StringCellValue);
                        }
                        else
                        {
                            if (endLine > 0)
                            {
                                result.Add(string.Empty);
                            }
                            else
                            {
                                return result;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.unityLogger.LogException(e);
                        Debug.unityLogger.LogError("ReadExcelError", "Comment " + i + " is Error");
                    }
                }
                return result;
            }
            else
            {
                Debug.unityLogger.LogError("GetClomnType", "行无效");
                return null;
            }
        }

        /// <summary>
        /// 得到一列,如果未输入结束列则遇到空结束
        /// </summary>
        /// <param name="lineNum"></param>
        /// <returns></returns>
        public List<string> GetColume(int lineNum, int startRow = 0, int endRow = -1)
        {
            List<string> result = new List<string>();
            int lastRowNum = sheet.LastRowNum;
            if (endRow > 0)
                lastRowNum = endRow;
            for (int row = startRow; row <= lastRowNum; row++)
            {
                try
                {
                    if (sheet.GetRow(row) == null)
                    {
                        if (endRow < 0)
                        {
                            return result;
                        }
                        else
                        {
                            result.Add(string.Empty);
                            continue;
                        }
                    }

                    sheet.GetRow(row).Cells[lineNum].SetCellType(CellType.String);
                    string total = sheet.GetRow(row).GetCell(lineNum).StringCellValue.Trim();
                    if (string.IsNullOrEmpty(total))
                    {
                        if (endRow > 0)
                        {
                            result.Add(string.Empty);
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        result.Add(total);
                    }
                }
                catch (Exception e)
                {
                    Debug.unityLogger.LogException(e);
                    Debug.unityLogger.LogError("ReadExcelError", "row " + row + " is Error");
                }
            }
            return result;
        }

        /// <summary>
        /// 获得每一行数据
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetRowObjs(int start = 2)
        {
            List<string> clomnNameList = GetMemberNames();
            List<Type> typeList = GetMemberTypes();
            int length = clomnNameList.Count;
            List<string> tagName = new List<string>();
            for (int i = 0; i < length; i++)
            {
                tagName.Add(clomnNameList[i]);
            }

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            int current = 0;
            foreach (IRow row in sheet)
            {
                if (current < start)
                {
                    current++;
                    continue;
                }

                Dictionary<string, object> rowDict = new Dictionary<string, object>();
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        row.GetCell(i).SetCellType(CellType.String);
                        string cellStr = row.GetCell(i).StringCellValue.Trim();
                        if (!string.IsNullOrEmpty(cellStr))
                        {
                            rowDict.Add(tagName[i], row.GetCell(i).StringCellValue.GetValue(typeList[i]));
                        }
                        else
                        {
                            Debug.unityLogger.LogError("ReadExcelError", string.Format("Value in Posistion row[{0}] line[{1}] is Error", current, i));
                            return rows;
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.unityLogger.LogException(e);
                        Debug.unityLogger.LogError("ReadExcelError", string.Format("Value in Posistion row[{0}] line[{1}] is Error", current, i));
                    }

                }
                rows.Add(rowDict);
                current++;
            }

            return rows;
        }

        #endregion
    }

}
