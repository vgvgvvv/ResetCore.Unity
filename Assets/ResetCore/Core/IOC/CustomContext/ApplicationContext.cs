using System.Collections;
using System.Collections.Generic;
using ResetCore.Asset;
using ResetCore.NAsset;
using UnityEngine;

namespace ResetCore.IOC
{
    /// <summary>
    /// 全局上下文，包含了包括加载等等核心模块
    /// </summary>
    public class ApplicationContext : CodeContext<ApplicationContext>
    {
        [Bean]
        public IBundleLoader GetBundleLoader()
        {
            return new DefaultLoader();
        }
        
    }
}
