using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ResetCore.Util;

namespace ResetCore.IOC
{
    public class Inject : Attribute
    {
        /// <summary>
        /// 是否为单例
        /// </summary>
        public bool isSingleton { get; private set; }

        public Inject(bool isSingleton = true)
        {
            this.isSingleton = isSingleton;
        }
    }

    public class InContext : Attribute
    {
        public Type[] contextTypes { get; private set; }
        public InContext(params Type[] contextTypes)
        {
            this.contextTypes = contextTypes;
        }
    }


    public abstract class IOCMonoBehavior : MonoBehaviour
    {
        /// <summary>
        /// 当前类的类型
        /// </summary>
        public Type behaviorType { get; private set; }

        /// <summary>
        /// 当前的类的属性
        /// </summary>
        public PropertyInfo[] properties { get; private set; }

        /// <summary>
        /// 当前类的域
        /// </summary>
        public FieldInfo[] fields { get; private set; }

        protected virtual void Awake()
        {
            behaviorType = GetType();
            var incontexts = behaviorType.GetCustomAttributes(typeof(InContext), true);
            for(int i = 0; i < incontexts.Length; i++)
            {
                var incontext = (InContext)incontexts[i];
                for (int j = 0; j < incontext.contextTypes.Length; j++)
                {
                    var contextType = typeof(Context<>);
                    contextType = contextType.MakeGenericType(incontext.contextTypes[j]);
                    var prop = contextType.GetProperty("context");

                    Context context = prop.GetValue(null, null) as Context;
                    InjectMember(context);
                }
            }
            
        }

        /// <summary>
        /// 注入成员
        /// </summary>
        private void InjectMember(Context context)
        {
           

            properties = behaviorType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for(int i = 0; i < properties.Length; i++)
            {
                var injectAttr = properties[i].GetFirstAttribute<Inject>(true);
                if (properties[i].GetSetMethod() != null 
                    && injectAttr != null)
                {
                    if (injectAttr.isSingleton)
                        properties[i].SetValue(this, context.GetSingleton(properties[i].PropertyType), new object[0]);
                    else
                        properties[i].SetValue(this, context.GetNew(properties[i].PropertyType), new object[0]);
                }
            }

            fields = behaviorType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for(int i = 0; i < fields.Length; i++)
            {
                var injectAttr = fields[i].GetFirstAttribute<Inject>(true);
                if (injectAttr != null)
                {
                    if (injectAttr.isSingleton)
                        fields[i].SetValue(this, context.GetSingleton(fields[i].FieldType));
                    else
                        fields[i].SetValue(this, context.GetNew(fields[i].FieldType));

                }
            }
        }
    }

}
