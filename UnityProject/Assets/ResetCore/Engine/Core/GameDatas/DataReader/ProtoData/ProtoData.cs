using UnityEngine;
using System.Collections;
using System.IO;

public class ProtoData<T> where T : class, ProtoBuf.IExtensible {

    T[] m_dataItems;
    public ProtoData()
    {
        byte[] buf = Resources.Load<TextAsset>(PathConfig.GetLocalGameDataResourcesPath(PathConfig.DataType.Protobuf)
            + typeof(T).ToString().Split('.')[1] + "protodata").bytes;

        MemoryStream ms = new MemoryStream(buf);
        BinaryReader br = new BinaryReader(ms);

        string typename = br.ReadString() + ",m-client-proto";
        int size = br.ReadInt32();
        m_dataItems = new T[size];

        System.Type type = System.Type.GetType(typename);
        if (!typeof(T).IsAssignableFrom(type))
        {
            Debug.LogError("Type does not matched");
            return;
        }

        for (int i = 0; i < size; i++)
        {
            int len = br.ReadInt32();
            byte[] itemBuf = br.ReadBytes(len);

            m_dataItems[i] = ProtoBuf.Serializer.NonGeneric.Deserialize(type, new MemoryStream(itemBuf)) as T;

        }
    }

    public int Count
    {
        get { return m_dataItems.Length; }
    }

    public T this[int index]
    {
        get { return m_dataItems[index]; }
    }
}
