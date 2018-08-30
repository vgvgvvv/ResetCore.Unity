using System;
using System.Collections;
using System.Reflection;

namespace ResetCore.Util
{

    /// 
    /// 对传入的对象进行深复制
    /// Cloner which do deep copy of an array or class instance.
    /// 
    /// See Also:
    /// 	http://www.codeproject.com/Articles/38270/Deep-copy-of-objects-in-C
    /// 
    /// CAUTION:
    /// 	For an inheritance case, deep copying all member fields of the ancestor are not tested.
    /// 	对于继承的情况，深复制对于所有父类域成员没有进行测试
    /// 	
    public class Cloner
    {
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null");
            return (T)Process(obj);
        }

        static object Process(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                //获取数组成员类型
                Type elementType = Type.GetType(
                     type.FullName.Replace("[]", string.Empty));

                //创建数组成员对象
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                
                //对数组成员进行赋值
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                
                //全部赋值
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, Process(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }

    }
}