using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using ResetCore.Xml;
using System;
using System.Xml.Serialization;

namespace ResetCore.PlatformHelper
{
    [XmlRoot("BuildFile")]
    public sealed class BuildFile
    {

        /// <summary>
        ///  包名
        /// </summary>
        [XmlElement("packageName")]
        public string packageName { get; set; }

        /// <summary>
        /// 最小SDK版本号
        /// </summary>
        [XmlElement("minSdkVersion")]
        public int minSdkVersion { get; set; }

        /// <summary>
        /// 目标SDK版本号
        /// </summary>
        [XmlElement("targetSdkVersion")]
        public int targetSdkVersion { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        [XmlElement("appName")]
        public string appName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [XmlElement("versionCode")]
        public string versionCode { get; set; }

        /// <summary>
        /// 版本名
        /// </summary>
        [XmlElement("versionName")]
        public string versionName { get; set; }

        /// <summary>
        /// 配置数据
        /// </summary>
        //[XmlElement("metaDatas")]
        //public Dictionary<string, string> metaDatas { get; set; }
    }

}
