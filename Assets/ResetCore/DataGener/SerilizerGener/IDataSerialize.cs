using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace ResetCore.Data
{
    public interface IDataSerialize
    {
        void ToXml(object data, XmlElement xml);
        object FromXml(XmlElement xml);

        void ToBinary(object data, BinaryWriter bw);
        object FromBinary(BinaryReader br);

        string ToString(object data);
        object FromString(string text);
    }

}
