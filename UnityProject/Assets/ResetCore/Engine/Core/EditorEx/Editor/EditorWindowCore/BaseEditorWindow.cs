using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorEx{
    
	public abstract class BaseEditorWindow<T> : EditorWindow where T : BaseEditorWindow<T>
	{
        /// <summary>
        /// 窗口本体
        /// </summary>
        /// <value>The window.</value>
        public T window { get; protected set; }

        /// <summary>
        /// 窗口名
        /// </summary>
        /// <value>The name of the window.</value>
        public string windowName { get; set; }

        /// <summary>
        /// EditorWindow根节点
        /// </summary>
        /// <value>The panel.</value>
        public EditorWindowPanel mainPanel { get; private set; }


        /// <summary>
        /// 展示主窗口
        /// </summary>
		public static T ShowMainWindow()
		{
            var newWindow = ScriptableObject.CreateInstance(typeof(TestEditorUI)) as T;
            newWindow.window = GetWindow(typeof(T), true, newWindow.windowName ?? typeof(T).Name) as T;
            newWindow.mainPanel = new EditorWindowPanel();
            newWindow.OnInit();
			newWindow.window.Show();
            return newWindow.window;
		}


		void OnGUI()
		{
            if (mainPanel == null)
                return;
            mainPanel.viewRect = new Rect(0, 0, position.width, position.height);
            mainPanel.OnGUI();
		}

        /// <summary>
        /// 对窗口进行初始化
        /// </summary>
        protected virtual void OnInit(){
            
        }

	}
}

