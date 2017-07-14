using ExcelDataManager.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //string excelPath = @"E:\Unity5Project\U5ABTest\C#Tools\test\";
            //string localGameDataXmlPath = @"E:\Unity5Project\U5ABTest\C#Tools\test\";
            //string localGameDataClassPath = @"E:\Unity5Project\U5ABTest\C#Tools\test\";
            //try
            //{
            //    GameDataGener.GenXmlAndGameData(excelPath, localGameDataXmlPath, localGameDataClassPath);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message + " " + e.StackTrace);
                
            //}
            //Console.ReadLine();
            if (args.Length >= 1)
            {
                if (args[0] == "GameDataGen")
                {
                    GameDataGener.GameDataGenMain(args);
                }
                //else if(
                //{

//}
                else
                {
                    Console.WriteLine("未知的操作命令");
                    Console.ReadLine();
                }
            }
            else 
            {
                Console.WriteLine("你倒是给我参数啊");
            }
            Console.WriteLine("结束");
            Console.ReadLine();
        }
    }
}
