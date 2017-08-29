using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
public class TestProtoClass {

    [ProtoMember(1, IsRequired = true)]
    public List<string> dependenies { get; set; }
}
