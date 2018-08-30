using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace ResetCore.Protobuf
{
    public class ProtoEx
    {
        /// <summary>
        /// 从Bytes中读取类 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T Read<T>(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            return ProtoBuf.Serializer.Deserialize<T>(ms);
        }

        /// <summary>
        /// 将类写入到路径下的文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void Write(object obj, string path)
        {
            using (var file = System.IO.File.Create(path))
            {
                ProtoBuf.Serializer.NonGeneric.Serialize(file, obj);
            }
        }


        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T model)
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中
                using (MemoryStream ms = new MemoryStream())
                {
                    //使用ProtoBuf工具的序列化方法
                    ProtoBuf.Serializer.Serialize<T>(ms, model);
                    //定义二级制数组，保存序列化后的结果
                    byte[] result = new byte[ms.Length];
                    //将流的位置设为0，起始点
                    ms.Position = 0;
                    //将流中的内容读取到二进制数组中
                    ms.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("序列化失败: " + ex.ToString());
                return null;
            }
        }

        // 将收到的消息反序列化成对象
        // < returns>The serialize.< /returns>
        // < param name="msg">收到的消息.</param>
        public static T DeSerialize<T>(byte[] msg)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中
                    ms.Write(msg, 0, msg.Length);
                    //将流的位置归0
                    ms.Position = 0;
                    //使用工具反序列化对象
                    T result = ProtoBuf.Serializer.Deserialize<T>(ms);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
                Debug.unityLogger.LogError("Protobuf", "反序列化失败: " + ex.ToString());
                return default(T);
            }
        }
    }

}


