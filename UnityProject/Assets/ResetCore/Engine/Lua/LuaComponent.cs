//#if LUA
//using UnityEngine;
//using System.Collections;
//using LuaInterface;

//namespace ResetCore.Lua
//{
//    [AddComponentMenu("Toos/Lua/LuaComponent")]
//    public class LuaComponent : MonoBehaviour
//    {

//        [Tooltip("绑定的LUA脚本路径")]
//        public TextAsset m_luaScript;
//        public LuaTable LuaModule
//        {
//            get;
//            private set;
//        }
//        LuaFunction m_luaUpdate;    // Lua实现的Update函数，可能为null



//        /// <summary>
//        /// 找到游戏对象上绑定的LUA组件（Module对象）
//        /// </summary>
//        public static LuaTable GetLuaComponent(GameObject go)
//        {
//            LuaComponent luaComp = go.GetComponent<LuaComponent>();
//            if (luaComp == null)
//                return null;
//            return luaComp.LuaModule;
//        }

//        /// <summary>
//        /// 向一个GameObject添加一个LUA组件
//        /// </summary>
//        public static LuaTable AddLuaComponent(GameObject go, TextAsset luaFile)
//        {
//            LuaComponent luaComp = go.AddComponent<LuaComponent>();
//            luaComp.Initilize(luaFile);  // 手动调用脚本运行，以取得LuaTable返回值
//            return luaComp.LuaModule;
//        }

//        /// <summary>
//        /// 提供给外部手动执行LUA脚本的接口
//        /// </summary>
//        public void Initilize(TextAsset luaFile)
//        {
//            m_luaScript = luaFile;
//            RunLuaFile(luaFile);

//            //-- 取得常用的函数回调
//            if (this.LuaModule != null)
//            {
//                m_luaUpdate = this.LuaModule["Update"] as LuaFunction;
//            }
//        }

//        /// <summary>
//        /// 调用Lua虚拟机，执行一个脚本文件
//        /// </summary>
//        void RunLuaFile(TextAsset luaFile)
//        {
//            if (luaFile == null || string.IsNullOrEmpty(luaFile.text))
//            {
//                Debug.unityLogger.LogError("LuaComponent", gameObject.name + "绑定的组件内容为空");
//                return;
//            }

//            object[] luaRet = LuaManager.luaScrMgr.DoString(luaFile.text);
//            if (luaRet != null && luaRet.Length >= 1)
//            {
//                // 约定：第一个返回的Table对象作为Lua模块
//                this.LuaModule = luaRet[0] as LuaTable;
//            }
//            else
//            {
//                Debug.unityLogger.LogError("LuaComponent", "Lua脚本没有返回Table对象：" + luaFile.name);
//            }
//        }

//        // MonoBehaviour callback
//        void Awake()
//        {
//            Initilize(m_luaScript);
//            CallLuaFunction("Awake", this.LuaModule, this.gameObject);
//        }

//        // MonoBehaviour callback
//        void Start()
//        {
//            CallLuaFunction("Start", this.LuaModule, this.gameObject);
//        }

//        // MonoBehaviour callback
//        void Update()
//        {
//            if (m_luaUpdate != null)
//                m_luaUpdate.Call(this.LuaModule, this.gameObject);
//        }

//        /// <summary>
//        /// 调用一个Lua组件中的函数
//        /// </summary>
//        void CallLuaFunction(string funcName, params object[] args)
//        {
//            if (this.LuaModule == null)
//                return;

//            LuaFunction func = this.LuaModule[funcName] as LuaFunction;
//            if (func != null)
//                func.Call(args);
//        }
//    }

//}
//#endif