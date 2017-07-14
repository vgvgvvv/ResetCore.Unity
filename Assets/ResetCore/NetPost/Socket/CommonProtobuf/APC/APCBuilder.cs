using System.Collections;
using Protobuf.Data;
using ResetCore.Util;
using System.Collections.Generic;
using System;

namespace ResetCore.NetPost
{
    public class APCBuilder
    {
        /// <summary>
        /// 构建APCData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="functionName"></param>
        /// <param name="args"></param>
        public static APCData BuildAPC(int id, params object[] args)
        {
            APCData data = new APCData();
            data.Id = id;
            foreach (object obj in args)
            {
                if (SendByAPC(obj))
                {
                    data.Content.Add(BitConverterEx.GetBytes(obj));
                }
                else if(obj is byte[])
                {
                    data.Content.Add((byte[])obj);
                }
                else
                {
                    throw new Exception("只能将值类型传入到apc中");
                }
            }
            return data;
        }

        /// <summary>
        /// 是否可以通过APC发送
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool SendByAPC(object obj)
        {
            if(obj is char || obj is bool || obj is int || obj is long || obj is short 
                || obj is uint || obj is ulong || obj is ushort || obj is float || obj is double)
            {
                return true;
            }
            return false;
        }
    }
}
