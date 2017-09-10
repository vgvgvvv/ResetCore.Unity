using UnityEngine;
using System.Collections;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using ResetCore.Util;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace ResetCore.Excel
{
    public class ExcelWriter
    {
        public string filePath { get; private set; }
        public string SheetName { get; private set; }
        public IWorkbook workbook { get; private set; }
        public ISheet sheet { get; private set; }

        public ExcelWriter(string filePath, string SheetName)
        {
            this.filePath = filePath.Replace("\\", "/");
            this.SheetName = SheetName;
        }

        public void WriteLine(int lineNum, object[] obj)
        {
            //TODO
        }

        public void WriteRow(int rowNum, object[] obj)
        {
            //TODO
        }

        public void Write(int line, int row, object obj)
        {
            ICell cell = sheet.GetRow(row).GetCell(line);
            cell.SetCellType(CellType.String);
            cell.SetCellValue(obj.ConverToString());
        }

        public void CreateFile()
        {
            //TODO

            //PathEx.MakeDirectoryExist(Path.GetDirectoryName(filePath));
            ////FileStream fs = File.Create(filePath);
            //this.workbook = new HSSFWorkbook();
            //this.sheet = workbook.CreateSheet(SheetName);

            //using (FileStream fs = File.Create(filePath))
            //{
            //    workbook.Write(fs);//向打开的这个xls文件中写入并保存。  
            //}
        }


    }

}
