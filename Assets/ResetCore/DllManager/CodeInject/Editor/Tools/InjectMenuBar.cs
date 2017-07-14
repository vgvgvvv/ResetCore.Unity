using ResetCore.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ResetCore.ReAssembly
{
    public class InjectMenuBar
    {
        [MenuItem("Tools/DllManager/Inject/FixDll")]
        public static void FixDll()
        {
            string scriptFolder = PathEx.Combine(PathConfig.projectPath, "Library/ScriptAssemblies");
            Directory.Delete(scriptFolder, true);
            EditorApplication.OpenProject(PathConfig.projectPath);
        }

    }

}
