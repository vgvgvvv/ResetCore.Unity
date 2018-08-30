//#if LUA
//using UnityEngine;
//using System.Collections;
//using ResetCore.Util;
//using LuaInterface;
//using ResetCore.Asset;
//using System.Collections.Generic;
//using ResetCore.NAsset;

//namespace ResetCore.Lua
//{
//    public class LuaManager : Singleton<LuaManager>
//    {

//        private static LuaScriptMgr _luaScrMgr; // 全局的Lua虚拟机
//        public static LuaScriptMgr luaScrMgr
//        {
//            get
//            {
//                if (_luaScrMgr == null)
//                {
//                    _luaScrMgr = new LuaScriptMgr();
//                    _luaScrMgr.Start();
//                }
//                return _luaScrMgr;
//            }
//        }

//        private Dictionary<string, string> luaDict = new Dictionary<string, string>();
//        private Dictionary<string, LuaTable> luaTableDict = new Dictionary<string, LuaTable>();

//        /// <summary>
//        /// 运行Lua
//        /// </summary>
//        /// <param name="fileName"></param>
//        public void DoLua(string bundleName, string fileName)
//        {
//            if (luaDict.ContainsKey(fileName))
//            {
//                luaScrMgr.DoString(luaDict[fileName]);
//            }
//            else
//            {
//                TextAsset textAsset = AssetLoader.GetText(bundleName, fileName);
//                luaScrMgr.DoString(textAsset.text);
//                luaDict.Add(fileName, textAsset.text);
//            }
//        }

//        /// <summary>
//        /// 加载Lua内的模块
//        /// </summary>
//        /// <param name="fileName">文件名</param>
//        /// <returns></returns>
//        public LuaTable LoadLua(string bundleName, string fileName)
//        {
//            if (luaTableDict.ContainsKey(fileName))
//            {
//                Debug.unityLogger.Log("重复利用" + fileName);
//                return luaTableDict[fileName];
//            }
//            else
//            {
//                Debug.unityLogger.Log("加载" + fileName);
//                luaTableDict.Add(fileName, null);
//                TextAsset textAsset = AssetLoader.GetText(bundleName, fileName);
//                luaDict.Add(fileName, textAsset.text);
//                LuaTable table = luaScrMgr.DoString(textAsset.text)[0] as LuaTable;
//                luaTableDict[fileName] = table;
//                return table;
//            }

//        }
//        /// <summary>
//        /// 调用Lua中的函数
//        /// </summary>
//        /// <param name="fileName">文件名</param>
//        /// <param name="funcName">函数明显</param>
//        /// <param name="args">参数</param>
//        public void Call(string bundleName, string fileName, string funcName, params object[] args)
//        {
//            if (!luaTableDict.ContainsKey(fileName))
//            {
//                LoadLua(bundleName, fileName);
//            }

//            LuaFunction func = this.luaTableDict[fileName][funcName] as LuaFunction;

//            if (func != null)
//            {
//                object[] args2 = new object[args.Length + 1];
//                args2[0] = this.luaTableDict[fileName];
//                for (int i = 1; i < args2.Length; i++)
//                {
//                    args2[i] = args[i - 1];
//                }
//                func.Call(args);
//            }
                
//        }

//        public void Call(string bundleName, string fileName, string funcName, object arg)
//        {
//            if (!luaTableDict.ContainsKey(fileName))
//            {
//                LoadLua(bundleName, fileName);
//            }

//            LuaFunction func = this.luaTableDict[fileName][funcName] as LuaFunction;

//            if (func != null)
//            {
//                func.Call(this.luaTableDict[fileName], arg);
//            }

//        }

//        public T GetValue<T>(string bundleName, string fileName, string valueName)
//        {
//            if (!luaTableDict.ContainsKey(fileName))
//            {
//                LoadLua(bundleName, fileName);
//            }
//            //Debug.logger.Log(luaTableDict[fileName][valueName].RefObject().GetType().Name);
//            return (T)(luaTableDict[fileName][valueName].RefObject());
//        }
//    }
//}

//#endif