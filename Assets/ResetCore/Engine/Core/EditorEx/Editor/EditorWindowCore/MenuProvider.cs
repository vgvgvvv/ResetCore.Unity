using UnityEngine;
using UnityEditor;

namespace EditorEx{
    
    public class MenuProvider : EditorWindowBehavior
	{
		/// <summary>
		/// 可以点击的Rect范围
		/// </summary>
        private EditorWindowRect parentWindow;

        public GenericMenu menu { get; set; }

        public MenuProvider(EditorWindowRect parent)
		{
			this.parentWindow = parent;
            menu = new GenericMenu();
            SetParent(parent);
		}

		public override void OnContextClick(Event currentEvent)
		{
            if (parentWindow.viewRect.Contains(currentEvent.mousePosition))
			{
				Vector2 mousePos = currentEvent.mousePosition;
				menu.ShowAsContext();
				currentEvent.Use();
			}
		}

	}

}
