using UnityEngine;
using System.Collections;
using ResetCore.NAsset;
using System.Xml.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using ResetCore.Event;
using ResetCore.Util;


namespace ResetCore.BehaviorTree
{
    [AddComponentMenu("BehaviorTree/BehaviorRoot")]
    public class BehaviorRoot : MonoBehaviour
    {

        //行为树R文件
        [SerializeField]
        private string behaviorTreeInfoBundlePath;
        [SerializeField]
        private string behaviorTreeInfoName;

        //当触发这些事件时会进行Tick
        [SerializeField]
        private List<string> tickEventsList;

        private BaseBehaviorNode rootBehavior;


        public ActionNode currentRunningNode { get; set; }

        public ActionQueue actionQueue { get; private set; }

        void Awake()
        {
            LoadBehaviorTreeInfo();

            foreach (string eventName in tickEventsList)
            {
                EventDispatcher.AddEventListener(eventName, Tick, gameObject);
            }
        }

        void OnDestroy()
        {
            foreach (string eventName in tickEventsList)
            {
                EventDispatcher.RemoveEventListener(eventName, Tick, gameObject);
            }
        }


        void Start()
        {
            Tick();
        }

        private void LoadBehaviorTreeInfo()
        {
            string xmlStr = AssetLoader.GetText(behaviorTreeInfoBundlePath, behaviorTreeInfoName).text;
            XDocument xDoc = XDocument.Parse(xmlStr);

            string rootBehaviorName = xDoc.Root.Name.LocalName;
            rootBehavior = BaseBehaviorNode.Getbehavior(rootBehaviorName);
            rootBehavior.root = this;
            LoadBehaviorList(xDoc.Root, rootBehavior);
        }

        private void LoadBehaviorList(XElement parentEl, BaseBehaviorNode parentBehavior)
        {
            if (!parentEl.HasElements) return;

            BaseBehaviorNode childBehavior;

            foreach (XElement el in parentEl.Elements())
            {

                childBehavior = BaseBehaviorNode.Getbehavior(el.Name.LocalName);
                parentBehavior.AddChild(childBehavior);

                if (el.HasElements)
                {
                    LoadBehaviorList(el, childBehavior);
                }
            }
        }

        public void Tick()
        {
            actionQueue.Clean();
            currentRunningNode.StopBehavior();
            rootBehavior.DoBehavior();
        }
    }

}

