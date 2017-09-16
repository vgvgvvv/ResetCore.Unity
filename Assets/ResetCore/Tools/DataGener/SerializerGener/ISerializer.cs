using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace ResetCore.Data
{
    public interface ISerializer<T>
    {
        void ToXElement(XmlElement xElement, string name, T obj);

        T ParseXElement(XmlElement xele, string name);
    }
}
