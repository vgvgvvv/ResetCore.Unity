using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorEx{

    public abstract class EditorWindowSubWindow : EditorWindowRect
	{

		/// <summary>
		/// 唯一Id
		/// </summary>
		public int id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        /// <value>The title.</value>
        public string title { get; set; }

        public override void OnGUI()
        {
            base.OnGUI();
            viewRect = GUILayout.Window(id, viewRect, DoWindow, title);
        }

		/// <summary>
		/// 绘制窗口
		/// </summary>
		/// <param name="unusedWindow"></param>
        protected virtual void DoWindow(int unusedWindow){
            
        }
		
	}


}
