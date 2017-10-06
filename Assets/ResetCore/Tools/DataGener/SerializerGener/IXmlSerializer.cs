using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace ResetCore.Data
{
    public interface IXmlSerializer<T>
    {
        void Write(XmlElement xElement, string name, T obj);

        T Read(XmlElement xele, string name);
    }
}
