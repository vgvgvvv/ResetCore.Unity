using UnityEngine;
using System.Collections;
using ResetCore.Util;
using ResetCore.ModuleControl;

namespace ResetCore.ReAssembly
{
    public static class DllManagerConst
    {
        public static readonly string scriptableCSOutputPath = PathEx.Combine(ModuleConst.GetSymbolPath(MODULE_SYMBOL.DLLMANAGER), "SciptableLoader/ScriptableObjects");
        public static readonly string scriptableObjectOutputPath = PathEx.Combine(PathConfig.resourcePath, PathConfig.compInfoObjRootPath);
    }

}
