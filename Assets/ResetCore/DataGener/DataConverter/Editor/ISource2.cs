using UnityEngine;
using System.Collections;

namespace ResetCore.Data
{
    public interface ISource2
    {
        DataType dataType { get; }
        void GenData(IDataReadable reader, string outputPath = null);
        void GenCS(IDataReadable reader);
    }

}
