using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelDataManager.Code
{
    public class ProtoBufGener
    {
        public static void GenProtoBufClass(string excelPath, string finalPath)
        {
            ParseXlsx.OpenExcelApplication();
            Excel._Worksheet sheet = ParseXlsx.OpenExcelWorkBook(excelPath).OpenExcelSheet();

            string className = sheet.Name;

            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace theNamespace = new CodeNamespace("ResetCore.Data.Protobuf");
            unit.Namespaces.Add(theNamespace);
        }
    }
}
