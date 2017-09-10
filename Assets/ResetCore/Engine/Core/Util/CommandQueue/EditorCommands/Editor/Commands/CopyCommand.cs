using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ResetCore.Util.CommandQueue.EditorCommand
{
    public class CopyCommand : BaseCommand
    {
        /// <summary>
        /// 要拷贝的文件位置
        /// </summary>
        public string[] from;
        
        /// <summary>
        /// 要粘贴的位置
        /// </summary>
        public string to;

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool ifOverwrite;

        public override void AddToQueue(ActionQueue queue)
        {
            queue.AddAction(Execute);
        }

        public override string Description()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("从\n");
            foreach (string fileName in from)
            {
                stringBuilder.Append(fileName).Append("\n");
            }
            stringBuilder.Append("拷贝到\n");
            stringBuilder.Append(to);
            return stringBuilder.ToString();
        }

        public override void Execute(Action act)
        {
            foreach(string fileName in from)
            {
                if (!File.Exists(fileName))
                    continue;

                string toFile = PathEx.Combine(to, Path.GetFileName(fileName));
                File.Copy(fileName, toFile);
                Debug.Log("将" + fileName + "拷贝到" + toFile);
            }
            act();
        }
    }

}
