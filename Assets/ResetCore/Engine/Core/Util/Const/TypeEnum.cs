using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum TypeEnum
{
    Int,
    String,
    Float,
    Double,
    Bool,
    Vector2,
    Vector3,
    Vector4,
    Color,
}

public static class TypeConst {

    public static Dictionary<TypeEnum, Type> TypeDict = new Dictionary<TypeEnum, Type>(){
        {TypeEnum.Int, typeof(int)},
        {TypeEnum.String, typeof(string)},
        {TypeEnum.Float, typeof(float)},
        {TypeEnum.Double, typeof(double)},
        {TypeEnum.Bool, typeof(bool)},
        {TypeEnum.Vector2, typeof(Vector2)},
        {TypeEnum.Vector3, typeof(Vector3)},
        {TypeEnum.Vector4, typeof(Vector4)},
        {TypeEnum.Color, typeof(Color)},
    };

    public static Dictionary<Type, TypeEnum> TypeEnumDict = new Dictionary<Type, TypeEnum>(){
        {typeof(int), TypeEnum.Int},
        {typeof(string), TypeEnum.String},
        {typeof(float), TypeEnum.Float},
        {typeof(double), TypeEnum.Double},
        {typeof(bool), TypeEnum.Bool},
        {typeof(Vector2), TypeEnum.Vector2},
        {typeof(Vector3), TypeEnum.Vector3},
        {typeof(Vector4), TypeEnum.Vector4},
        {typeof(Color), TypeEnum.Color},
    };
}
