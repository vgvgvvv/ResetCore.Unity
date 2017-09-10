using UnityEngine;
using System.Collections;
using System.IO;

namespace ResetCore.Asset
{
    public class EncryptHelper
    {

        public static void Encrypt(string path, string outputPath, System.Action afterAct = null)
        {
            //Do
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            byte[] buff = new byte[fs.Length];
            fs.Read(buff, 0, (int)fs.Length);
            fs.Close();
            //加密
            buff = DoEncrypt(buff);

            FileStream cfs = new FileStream(outputPath, FileMode.Create);
            cfs.Write(buff, 0, buff.Length);
            buff = null;
            cfs.Close();
        }

        public static AssetBundle Decrypt(byte[] bytes)
        {
            //解密
            byte[] decryptedData = DoDecrypt(bytes);
            return AssetBundle.LoadFromMemory(decryptedData);
        }

        //进行加密
        private static byte[] DoEncrypt(byte[] bytes)
        {
            byte[] result = new byte[bytes.Length + 1];
            for (int i = 0; i < bytes.Length; i++)
            {
                result[i] = bytes[i];
            }
            result[result.Length - 1] = 0;
            return result;
        }

        //进行解密
        private static byte[] DoDecrypt(byte[] bytes)
        {
            byte[] result = new byte[bytes.Length - 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = bytes[i];
            }
            return result;
        }
    }

}
