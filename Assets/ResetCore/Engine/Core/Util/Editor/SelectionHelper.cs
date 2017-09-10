using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace ResetCore.Util
{
    public class SelectionHelper
    {
        /// <summary>
        /// 选择符合条件的资源
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string[] SelectWithCondition(System.Func<Object, bool> condition)
        {
            var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            var paths = (from s in selection
                         let path = AssetDatabase.GetAssetPath(s)
                         where condition(s)
                         select path).ToArray();
            return paths;
        }
    }

}
