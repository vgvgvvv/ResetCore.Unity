using UnityEngine;
using System.Collections;

namespace ResetCore.Util
{
    public interface IEncoder<T>
    {
        byte[] Encode(T data);

    }
}
