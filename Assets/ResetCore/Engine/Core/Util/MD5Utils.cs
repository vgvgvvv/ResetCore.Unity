using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Security.Cryptography;

public class MD5Utils {

    public static String BuildFileMd5(String filename)
    {
        String filemd5 = null;
        try
        {
            using (var fileStream = File.OpenRead(filename))
            {
                //UnityEditor.AssetDatabase
                var md5 = MD5.Create();
                var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                            
                //fileStream.Close();//流数据比较大，手动卸载 
                //fileStream.Dispose();
                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”               
                filemd5 = FormatMD5(fileMD5Bytes);
            }
        }
        catch (System.Exception ex)
        {
            Debug.unityLogger.LogException(ex);
        }
        return filemd5;
    }
    public static string FormatMD5(Byte[] data)
    {
        return System.BitConverter.ToString(data).Replace("-", "").ToLower();
    }
}
