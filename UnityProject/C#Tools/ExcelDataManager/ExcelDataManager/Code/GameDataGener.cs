using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelDataManager.Code
{
    class GameDataGener
    {
        //本地XML路径
        private static string localGameDataXmlPath;
        //本地Class路径
        private static string localGameDataClassPath;

        public static void GameDataGenMain(string[] args)
        {

            string excelPath = args[1];
            string localGameDataXmlPath = args[2];
            string localGameDataClassPath = args[3];
            GenXmlAndGameData(excelPath, localGameDataXmlPath, localGameDataClassPath);
            
        }

        public static void GenXmlAndGameData(string excelPath, string localGameDataXmlPath, string localGameDataClassPath)
        {
            GameDataGener.localGameDataXmlPath = localGameDataXmlPath;
            GameDataGener.localGameDataClassPath = localGameDataClassPath;

            if (!Directory.Exists(excelPath))
            {
                Directory.CreateDirectory(excelPath);
            }
            if (!Directory.Exists(localGameDataXmlPath))
            {
                Directory.CreateDirectory(localGameDataXmlPath);
            }
            if (!Directory.Exists(localGameDataClassPath))
            {
                Directory.CreateDirectory(localGameDataClassPath);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(excelPath);
            FileInfo[] fileInfos = dirInfo.GetFiles();

            ParseXlsx.OpenExcelApplication();
            try
            {
                foreach (FileInfo info in fileInfos)
                {
                    if (info.Extension == ".xlsx" && !info.Name.StartsWith("~$"))
                    {
                        Console.WriteLine("读取Excel" + info.FullName);
                        Excel._Worksheet sheet = ParseXlsx.OpenExcelWorkBook(info.FullName).OpenExcelSheet();
                        CreateNewXmls(Path.GetFileNameWithoutExtension(info.Name) + ".xml", sheet);
                        Console.WriteLine();
                    }
                }
                CreateNewClasses();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.StackTrace);
            }
            finally
            {
                ParseXlsx.CloseExcelApplication();
            }
            

            
        }

        #region 生成Xml
        private static void CreateNewXmls(string xmlName, Excel._Worksheet sheet)
        {
            XDocument xDoc = new XDocument();

            XElement root = new XElement("Root");
            xDoc.Add(root);

            List<string> itemNameList = new List<string>();

            int clomn = 1;
            while(true){

                if (sheet.ReadExcelCell(2, 1) != "id" || sheet.ReadExcelCell(3, 1) != "Int32")
                {
                    Console.WriteLine("第一个属性应该为id！");
                    return;
                }

                string name = sheet.ReadExcelCell(2, clomn);
                string type = sheet.ReadExcelCell(3, clomn);
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                {
                    break;
                }

                itemNameList.Add(name + "_" + type);
                clomn++;
            }

            int row = 4;
            bool hasNext = true;
            while (hasNext)
            {
                
                if (string.IsNullOrEmpty(sheet.ReadExcelCell(row, 1)))
                {
                    hasNext = false;
                    break;
                }
                else
                {
                    XElement item = new XElement("item");
                    for (int i = 1; i < itemNameList.Count; i++)
                    {

                        string itemName = itemNameList[i];
                        string value = sheet.ReadExcelCell(row, i + 1);
                        XElement el = new XElement(itemName);
                        el.Value = value;
                        item.Add(el);

                    }
                    row++;
                    root.Add(item);
                }
            }
            string outputFile = Path.Combine(localGameDataXmlPath, xmlName);
            Console.WriteLine("生成Xml" + outputFile);
            xDoc.Save(outputFile);
        }

        #endregion


        #region 生成Class


        private static void CreateNewClasses()
        {
            
            if (!Directory.Exists(localGameDataXmlPath))
            {
                Directory.CreateDirectory(localGameDataXmlPath);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(localGameDataXmlPath);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            CreateEveryFilesClasses(fileInfos);
        }

        private static void CreateEveryFilesClasses(FileInfo[] fileInfos)
        {
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Extension == ".xml")
                {
                    Console.WriteLine("正在生成" + fileInfo.Name + "的Class中...");
                    CreateNewClass(fileInfo.FullName.Replace("\\", "/"));
                }
            }
        }

        //
        private static string className;
        private static string baseClassName;
        private static string nameSpace;
        private static string[] importNameSpaces;
        private static string outputFile;
        

        private static void CreateNewClass(string filePath)
        {
            GetPropString(filePath);

            XDocument xDoc = LoadXml(filePath);

            CodeCompileUnit unit;
            CodeTypeDeclaration NewClass;
            CreateNewClass(out unit, out NewClass);

            AddProp(xDoc, NewClass);

            GenCSharp(outputFile, unit);

        }
        private static void GetPropString(string filePath)
        {
            className = Path.GetFileNameWithoutExtension(filePath);
            baseClassName = "GameData<" + className + ">";
            nameSpace = "ResetCore.Data.GameDatas";
            importNameSpaces = new string[]{
            "System","System.Collections.Generic"
        };
            if (!Directory.Exists(localGameDataClassPath))
            {
                Directory.CreateDirectory(localGameDataClassPath);
            }
            outputFile = Path.Combine(localGameDataClassPath, className + ".cs");
        }
        private static XDocument LoadXml(string filePath)
        {
            XDocument xDoc = XDocument.Load(filePath);
            if (xDoc == null)
            {
                Console.WriteLine("创建GameData: " + "没有成功加载Xml");
            }
            return xDoc;
        }
        private static void CreateNewClass(out CodeCompileUnit unit, out CodeTypeDeclaration NewClass)
        {
            unit = new CodeCompileUnit();
            CodeNamespace theNamespace = new CodeNamespace(nameSpace);
            unit.Namespaces.Add(theNamespace);


            NewClass = new CodeTypeDeclaration(className);
            theNamespace.Types.Add(NewClass);

            foreach (string ns in importNameSpaces)
            {
                CodeNamespaceImport import = new CodeNamespaceImport(ns);
                theNamespace.Imports.Add(import);
            }


            NewClass.TypeAttributes = TypeAttributes.Public;
            NewClass.BaseTypes.Add(baseClassName);
            NewClass.IsClass = true;

            CodeMemberField fileNameField = new CodeMemberField("String", "fileName");
            fileNameField.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            fileNameField.InitExpression = new CodeSnippetExpression("\"" + className + "\"");
            NewClass.Members.Add(fileNameField);
        }
        private static void AddProp(XDocument xDoc, CodeTypeDeclaration NewClass)
        {
            foreach (XElement el in xDoc.Root.Element("item").Elements())
            {
                string[] propAttrs = el.Name.LocalName.Split('_');
                if (propAttrs.Length < 2)
                {
                    Console.WriteLine("警告！生成数据类: " + "属性" + el.Name.LocalName + "不存在属性类型信息");
                    return;
                }
                string propName = propAttrs[0];
                string propType = propAttrs[1];
                string propComment = "";
                if (propAttrs.Length > 2)
                {
                    for (int i = 2; i < propAttrs.Length; i++)
                    {
                        propComment += propAttrs[i] + " ";
                    }
                }
                //添加字段

                CodeMemberField field = new CodeMemberField(propType, "_" + propName);
                field.Attributes = MemberAttributes.Private;

                NewClass.Members.Add(field);

                //添加属性
                CodeMemberProperty property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Name = propName;
                property.HasGet = true;
                property.HasSet = true;
                property.Type = new CodeTypeReference(propType);
                property.Comments.Add(new CodeCommentStatement(propComment));
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + propName)));
                property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + propName), new CodePropertySetValueReferenceExpression()));

                NewClass.Members.Add(property);
            }
        }
        private static void GenCSharp(string outputFile, CodeCompileUnit unit)
        {
            //生成代码
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            //缩进样式
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            //空行
            options.BlankLinesBetweenMembers = true;

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                Console.WriteLine("生成代码" + outputFile);
                provider.GenerateCodeFromCompileUnit(unit, sw, options);

            }
        }

        #endregion
    }
}
