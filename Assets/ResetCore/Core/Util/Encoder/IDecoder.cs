using UnityEngine;
using System.Collections;

namespace ResetCore.Util
{
    public interface IDecoder<T>
    {

        T Decode(byte[] data);

    }
}
