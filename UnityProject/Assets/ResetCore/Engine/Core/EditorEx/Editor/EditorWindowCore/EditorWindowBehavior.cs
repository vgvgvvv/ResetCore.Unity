using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.Debugger;

namespace EditorEx{
    
    /// <summary>
    /// 不存在表现的GUI对象
    /// </summary>
	public abstract class EditorWindowBehavior
	{
        /// <summary>
        /// 父对象
        /// </summary>
        /// <value>The parent.</value>
        public EditorWindowRect parent { get; protected set; }

        /// <summary>
        /// 子对象列表
        /// </summary>
        private List<EditorWindowBehavior> behaviorList = new List<EditorWindowBehavior>();

        /// <summary>
        /// 绘制GUI
        /// </summary>
		public virtual void OnGUI()
		{
			OnGUIEvent();

            for (int i = 0; i < behaviorList.Count; i ++){
                behaviorList[i].OnGUI();
            }
		}

		/// <summary>
		/// 添加新的GUI子对象
		/// </summary>
		/// <param name="parent">Bahavior.</param>
        public void SetParent(EditorWindowRect parent){

            if(parent == this){
                ReDebug.LogError(ReLogType.System, "EditorWIndowBehavior", "不能将自己作为自己的子对象");
                return;
            }

            if(this is EditorWindowPanel && !(parent is EditorWindowPanel)){
				ReDebug.LogError(ReLogType.System, "EditorWIndowBehavior", "Panel只能放入Panel下");
				return;
            }

            if (this.parent == parent)
                return;

            if(this.parent != null){
                this.parent.behaviorList.Remove(this);
            }

            this.parent = parent;
            if(this.parent != null){
				this.parent.behaviorList.Add(this);
			}
            
        }



		#region GUI事件部分
		/// <summary>
		/// 当触发GUI事件
		/// </summary>
		protected void OnGUIEvent()
		{
			Event currentEvent = Event.current;
			switch (currentEvent.type)
			{
				case EventType.MouseDown:
					OnMouseDown(currentEvent);
					break;
				case EventType.MouseUp:
					OnMouseUp(currentEvent);
					break;
				case EventType.MouseMove:
					OnMouseMove(currentEvent);
					break;
				case EventType.MouseDrag:
					OnMouseDrag(currentEvent);
					break;
				case EventType.KeyDown:
					OnKeyDown(currentEvent);
					break;
				case EventType.KeyUp:
					OnKeyUp(currentEvent);
					break;
				case EventType.Repaint:
					OnRepaint(currentEvent);
					break;
				case EventType.Layout:
					OnLayout(currentEvent);
					break;
				case EventType.DragUpdated:
					OnDragUpdated(currentEvent);
					break;
				case EventType.DragPerform:
					OnDragPerform(currentEvent);
					break;
				case EventType.DragExited:
					OnDragExited(currentEvent);
					break;
				case EventType.Ignore:
					OnIgnore(currentEvent);
					break;
				case EventType.Used:
					OnUsed(currentEvent);
					break;
				case EventType.ValidateCommand:
					OnValidateCommand(currentEvent);
					break;
				case EventType.ExecuteCommand:
					OnExecuteCommand(currentEvent);
					break;
				case EventType.ContextClick:
					OnContextClick(currentEvent);
					break;
				case EventType.MouseEnterWindow:
					OnMouseEnterWindow(currentEvent);
					break;
				case EventType.MouseLeaveWindow:
					OnMouseLeaveWindow(currentEvent);
					break;
			}
		}

		public virtual void OnMouseDown(Event currentEvent) { }

		public virtual void OnMouseUp(Event currentEvent) { }

		public virtual void OnMouseMove(Event currentEvent) { }

		public virtual void OnMouseDrag(Event currentEvent) { }

		public virtual void OnKeyDown(Event currentEvent) { }

		public virtual void OnKeyUp(Event currentEvent) { }

		public virtual void OnRepaint(Event currentEvent) { }

		public virtual void OnLayout(Event currentEvent) { }

		public virtual void OnDragUpdated(Event currentEvent) { }

		public virtual void OnDragPerform(Event currentEvent) { }

		public virtual void OnDragExited(Event currentEvent) { }

		public virtual void OnIgnore(Event currentEvent) { }

		public virtual void OnUsed(Event currentEvent) { }

		public virtual void OnValidateCommand(Event currentEvent) { }

		public virtual void OnExecuteCommand(Event currentEvent) { }

		public virtual void OnContextClick(Event currentEvent) { }

		public virtual void OnMouseEnterWindow(Event currentEvent) { }

		public virtual void OnMouseLeaveWindow(Event currentEvent) { }

		#endregion GUI事件部分


	}

}
