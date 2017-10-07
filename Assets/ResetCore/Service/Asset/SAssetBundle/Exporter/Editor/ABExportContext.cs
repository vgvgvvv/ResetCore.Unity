using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.SAsset
{
    public class ABExportContext
    {

        private readonly Dictionary<int, ABTarget> object2target = new Dictionary<int, ABTarget>();
        private readonly Dictionary<string, ABTarget> assetPath2target = new Dictionary<string, ABTarget>();

        /// <summary>
        /// 所有打包目标
        /// </summary>
        public List<ABTarget> allTarget
        {
            get { return new List<ABTarget>(object2target.Values); }
        }

        /// <summary>
        /// 尝试获取ABTarget
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool TryGetTarget(int instanceId, out ABTarget target)
        {
            return object2target.TryGetValue(instanceId, out target);
        }

        /// <summary>
        /// 尝试获取ABTarget
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool TryGetTarget(string assetPath, out ABTarget target)
        {
            return assetPath2target.TryGetValue(assetPath, out target);
        }

        /// <summary>
        /// 添加Target
        /// </summary>
        /// <param name="target"></param>
        public void AddTarget(ABTarget target)
        {
            assetPath2target[target.assetPath] = target;
            object2target[target.asset.GetInstanceID()] = target;
        }

        /// <summary>
        /// 移除Target
        /// </summary>
        /// <param name="target"></param>
        public void RemoveTarget(ABTarget target)
        {
            assetPath2target.Remove(target.assetPath);
            object2target.Remove(target.asset.GetInstanceID());
        }
    }

}
