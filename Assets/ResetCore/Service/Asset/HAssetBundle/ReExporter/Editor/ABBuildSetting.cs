using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.SAsset
{
    public enum ABFormat
    {
        Text,
        Bin
    }

    public class ABBuildSetting
    {
        public ABFormat _binFormat = ABFormat.Bin;

    }
}
