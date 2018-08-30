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

    public class InCodeContext : Attribute
    {
        public Type[] contextTypes { get; private set; }
        public InCodeContext(params Type[] contextTypes)
        {
            this.contextTypes = contextTypes;
        }
    }

    public class InXmlContext : Attribute
    {
        public string[] contextPaths { get; private set; }

        public InXmlContext(params string[] contextPaths)
        {
            this.contextPaths = contextPaths;
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

            var inCodeContexts = behaviorType.GetCustomAttributes(typeof(InCodeContext), true);
            for(int i = 0; i < inCodeContexts.Length; i++)
            {
                var incontext = (InCodeContext)inCodeContexts[i];
                for (int j = 0; j < incontext.contextTypes.Length; j++)
                {
                    var contextType = typeof(CodeContext<>);
                    contextType = contextType.MakeGenericType(incontext.contextTypes[j]);
                    var prop = contextType.GetProperty("context");

                    CodeContext codeContext = prop.GetValue(null, null) as CodeContext;
                    InjectMember(codeContext);
                }
            }

            var inXmlContexts = behaviorType.GetCustomAttributes(typeof(InXmlContext), true);
            for (int i = 0; i < inXmlContexts.Length; i++)
            {
                var incontext = (InXmlContext)inCodeContexts[i];
                var paths = incontext.contextPaths;
                for (int j = 0; j < paths.Length; j++)
                {
                    var codeContext = XmlContext.GetContext(paths[j]);
                    InjectMember(codeContext);
                }
            }
        }

        /// <summary>
        /// 注入成员
        /// </summary>
        private void InjectMember(Context codeContext)
        {
            properties = behaviorType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for(int i = 0; i < properties.Length; i++)
            {
                var injectAttr = properties[i].GetFirstAttribute<Inject>(true);
                if (properties[i].GetSetMethod() != null 
                    && injectAttr != null)
                {
                    if (injectAttr.isSingleton)
                        properties[i].SetValue(this, codeContext.GetSingleton(properties[i].PropertyType), new object[0]);
                    else
                        properties[i].SetValue(this, codeContext.GetNewInstance(properties[i].PropertyType), new object[0]);
                }
            }

            fields = behaviorType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for(int i = 0; i < fields.Length; i++)
            {
                var injectAttr = fields[i].GetFirstAttribute<Inject>(true);
                if (injectAttr != null)
                {
                    if (injectAttr.isSingleton)
                        fields[i].SetValue(this, codeContext.GetSingleton(fields[i].FieldType));
                    else
                        fields[i].SetValue(this, codeContext.GetNewInstance(fields[i].FieldType));

                }
            }
        }
    }

}
