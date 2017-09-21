using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorEx{

    /// <summary>
    /// 存在表现的GUI对象，包含最基本的Rect以及贴图
    /// </summary>
	public abstract class EditorWindowRect : EditorWindowBehavior
	{
        /// <summary>
        /// 视图占用的Rect
        /// </summary>
        public Rect viewRect { get; set; }

        /// <summary>
        /// 用于渲染的Texture
        /// </summary>
        /// <value>The main texure.</value>
        public Texture mainTexure { get; set; }

        public EditorWindowRect(){
            this.viewRect = new Rect();
        }

        public EditorWindowRect(Rect viewRect){
            this.viewRect = viewRect;
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if(mainTexure != null)
			    GUI.DrawTexture(viewRect, mainTexure);
		}

	}

}
