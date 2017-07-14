using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using ResetCore.Util;

public static class CompressHelper
{
    /// <summary>
    /// 压缩目标目录
    /// </summary>
    /// <param name="sourcePath">压缩目录</param>
    /// <param name="outputFilePath">压缩文件输出路径</param>
    /// <param name="zipLevel"></param>
    public static void CompressDirectory(string sourcePath, string outputFilePath, int zipLevel = 0)
    {
        new FileStream(outputFilePath, FileMode.OpenOrCreate).CompressDirectory(sourcePath, zipLevel);
    }

    public static void CompressDirectory(this Stream target, string sourcePath, int zipLevel = 0)
    {
        sourcePath = Path.GetFullPath(sourcePath);
        int startIndex = string.IsNullOrEmpty(sourcePath) ? Path.GetPathRoot(sourcePath).Length : sourcePath.Length;
        List<string> list = new List<string>();
        list.AddRange(from d in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories) select d + @"\");
        list.AddRange(Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories));
        using (ZipOutputStream stream = new ZipOutputStream(target))
        {
            stream.SetLevel(zipLevel);
            foreach (string str in list)
            {
                string input = str.Substring(startIndex);
                string name = input.StartsWith(@"\") ? input.ReplaceFirst(@"\", "", 0) : input;
                name = name.Replace(@"\", "/");
                stream.PutNextEntry(new ZipEntry(name));
                Debug.Log(name);
                if (!str.EndsWith(@"\"))
                {
                    byte[] buffer = new byte[0x800];
                    using (FileStream stream2 = File.OpenRead(str))
                    {
                        int num2;
                        while ((num2 = stream2.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, num2);
                        }
                    }
                }
            }
            stream.Finish();
        }
    }

    /// <summary>
    /// 压缩目标目录中的文件
    /// </summary>
    /// <param name="sourcePath">压缩目录</param>
    /// <param name="filePath">需要压缩的文件</param>
    /// <param name="outputFilePath">目标目录</param>
    /// <param name="zipLevel"></param>
    public static void CompressFiles(string sourcePath, string[] filePath, string outputFilePath, int zipLevel = 0)
    {
        PathEx.MakeDirectoryExist(outputFilePath);
        Stream target = new FileStream(outputFilePath, FileMode.OpenOrCreate);
        sourcePath = Path.GetFullPath(sourcePath);
        int startIndex = string.IsNullOrEmpty(sourcePath) ? Path.GetPathRoot(sourcePath).Length : sourcePath.Length;
        using (ZipOutputStream stream = new ZipOutputStream(target))
        {
            stream.SetLevel(zipLevel);

            foreach (string str in filePath)
            {
                string input = str.Substring(startIndex).Replace(@"\", "/");
                string name = input.StartsWith(@"/") ? input.ReplaceFirst(@"/", "", 0) : input;
                stream.PutNextEntry(new ZipEntry(name));
                Debug.Log(name);
                if (!str.EndsWith(@"/"))
                {
                    byte[] buffer = new byte[0x800];
                    using (FileStream stream2 = File.OpenRead(str))
                    {
                        int num2;
                        while ((num2 = stream2.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, num2);
                        }
                    }
                }
            }

            stream.Finish();
        }
    }

    /// <summary>
    /// 解压文件到目标目录
    /// </summary>
    /// <param name="targetPath">目标目录</param>
    /// <param name="zipFilePath">压缩文件路径</param>
    public static void DecompressToDirectory(string targetPath, string zipFilePath)
    {
        if (File.Exists(zipFilePath))
        {
            File.OpenRead(zipFilePath).DecompressToDirectory(targetPath);
        }
        else
        {
            Debug.Log("Zip不存在: " + zipFilePath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetPath"></param>
    public static void DecompressToDirectory(this Stream source, string targetPath)
    {
        targetPath = Path.GetFullPath(targetPath);
        try
        {
            using (ZipInputStream stream = new ZipInputStream(source))
            {
                ZipEntry entry;
                while ((entry = stream.GetNextEntry()) != null)
                {
                    string name = entry.Name;
                    if (entry.IsDirectory && entry.Name.StartsWith(@"\"))
                    {
                        name = entry.Name.ReplaceFirst(@"\", "", 0);
                    }
                    string path = Path.Combine(targetPath, name);
                    string directoryName = Path.GetDirectoryName(path);
                    if (!(string.IsNullOrEmpty(directoryName) || Directory.Exists(directoryName)))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    if (!entry.IsDirectory)
                    {
                        byte[] buffer = new byte[0x800];
                        using (FileStream stream2 = File.Create(path))
                        {
                            int num;
                            while ((num = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                stream2.Write(buffer, 0, num);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("zip error is: " + exception.Message);
        }
    }

   
}
