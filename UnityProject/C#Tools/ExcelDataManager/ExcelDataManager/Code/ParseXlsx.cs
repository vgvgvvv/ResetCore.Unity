using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelDataManager.Code
{
    public static class ParseXlsx
    {
        private static Excel.Application excelFile;

        //public static StringBuilder classSource;
        //public static StringBuilder objectData;

        //public static void ReadExcelFile(string excelFilePath)
        //{
        //    classSource = new StringBuilder(); ;
        //    objectData = new StringBuilder();
        //    Excel._Workbook m_Workbook;
        //    Excel._Worksheet m_Worksheet;
        //    object missing = System.Reflection.Missing.Value;
        //    Console.WriteLine("excelFilePath:" + excelFilePath);
        //    excelFile.Workbooks.Open(excelFilePath);
        //    excelFile.Visible = false;
        //    m_Workbook = excelFile.Workbooks[1];
        //    m_Worksheet = (Excel.Worksheet)m_Workbook.ActiveSheet;
        //    int clomn_Count = m_Worksheet.UsedRange.Columns.Count;
        //    int row_Count = m_Worksheet.UsedRange.Rows.Count;
        //    classSource.Append("using System;\n");
        //    classSource.Append("[Serializable]\n");
        //    classSource.Append("public   class   DynamicClass \n");
        //    classSource.Append("{\n");
        //    string propertyName, propertyType;
        //    for (int j = 2; j < clomn_Count + 1; j++)
        //    {
        //        propertyName = ((Excel.Range)m_Worksheet.Cells[3, j]).Text.ToString();
        //        propertyType = ((Excel.Range)m_Worksheet.Cells[4, j]).Text.ToString();
        //        classSource.Append(" private  " + propertyType + "  _" + propertyName + " ;\n");
        //        classSource.Append(" public   " + propertyType + "   " + "" + propertyName + "\n");
        //        classSource.Append(" {\n");
        //        classSource.Append(" get{   return   _" + propertyName + ";}   \n");
        //        classSource.Append(" set{   _" + propertyName + "   =   value;   }\n");
        //        classSource.Append(" }\n");
        //        //classSource.Append("\tpublic " + ((Excel.Range)m_Worksheet.Cells[4, j]).Text.ToString() + " " + ((Excel.Range)m_Worksheet.Cells[3, j]).Text.ToString() + ";\n");
        //    }
        //    classSource.Append("}\n");
        //    Console.Write(classSource.ToString());
        //    for (int i = 7; i < row_Count + 1; i++)//
        //    {
        //        for (int j = 2; j < clomn_Count + 1; j++)
        //        {
        //            objectData.Append(((Excel.Range)m_Worksheet.Cells[i, j]).Text.ToString() + ";");

        //        }
        //        objectData.Append("\n");
        //        try
        //        {
        //            Console.Write(objectData.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.Write(ex.Message);
        //        }


        //    }
        //    //关闭Excel相关对象
        //    m_Worksheet = null;
        //    m_Workbook = null;
        //}

        public static Excel.Application OpenExcelApplication()
        {
            if (excelFile == null)
            {
                excelFile = new Excel.Application();
            }
            return excelFile;
        }

        public static Excel._Workbook OpenExcelWorkBook(string excelPath)
        {
            Excel._Workbook workbook;
            object missing = System.Reflection.Missing.Value;
            Console.WriteLine("excelFilePath:" + excelPath);
            excelFile.Workbooks.Open(excelPath);
            excelFile.Visible = true;
            workbook = excelFile.Workbooks[1];
            return workbook;
        }

        public static Excel._Worksheet OpenExcelSheet(string excelPath, int WorkbookIndex = -1)
        {
            Excel._Workbook workbook;
            Excel._Worksheet worksheet;
            object missing = System.Reflection.Missing.Value;
            Console.WriteLine("excelFilePath:" + excelPath);
            excelFile.Workbooks.Open(excelPath);
            excelFile.Visible = false;
            workbook = excelFile.Workbooks[1];
            if (WorkbookIndex != -1)
            {
                worksheet = (Excel.Worksheet)workbook.Sheets.get_Item(WorkbookIndex);
            }
            else
            {
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;
            }
            return worksheet;
        }

        public static Excel._Worksheet OpenExcelSheet(this Excel._Workbook workbook, int WorkbookIndex = -1)
        {
            Excel._Worksheet worksheet;
            object missing = System.Reflection.Missing.Value;
            if (WorkbookIndex != -1)
            {
                worksheet = (Excel.Worksheet)workbook.Sheets.get_Item(WorkbookIndex);
            }
            else
            {
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;
            }
            return worksheet;
        }

        public static string ReadExcelCell(this Excel._Worksheet worksheet, int row, int clomn)
        {
            return ((Excel.Range)worksheet.Cells[row, clomn]).Text.ToString();
        }

        public static void CloseExcelApplication()
        {
            excelFile.Quit();
        }
    }
}
