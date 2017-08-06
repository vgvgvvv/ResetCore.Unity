using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using ResetCore.Protobuf;
using LitJson;
using ResetCore.Util;

public class Proto2Yaml : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TestProtoClass test = new TestProtoClass();
        test.dependenies = new List<string>()
        {
            "123",
            "asd",
            "zxc",
            "dfg"
        };

        var bytes = ProtoEx.Serialize(test);
        var testFromProto = ProtoEx.DeSerialize<TestProtoClass>(bytes);
        Debug.Log(testFromProto.dependenies.ConverToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public string Yaml2Json(string yaml)
    {
        var r = new StringReader(yaml);
        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize(r);

        var serializer = new SerializerBuilder()
            .JsonCompatible()
            .Build();

        var json = serializer.Serialize(yamlObject);
        return json;
    }

    public byte[] Json2Proto<T>(string json)
    {
        var obj = JsonMapper.ToObject<T>(json);
        return ProtoEx.Serialize(obj);
    }

    public void ReadProto<T>(byte[] bytes)
    {
        TestProtoClass test = ProtoEx.Read<TestProtoClass>(bytes);
    }
}
