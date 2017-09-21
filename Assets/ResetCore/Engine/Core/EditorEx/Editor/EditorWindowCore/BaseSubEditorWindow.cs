using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace EditorEx{
	public abstract class BaseSubEditorWindow<T> where T : BaseSubEditorWindow<T>
	{

		/// <summary>
		/// 父窗口
		/// </summary>
		public EditorWindow parent { get; private set; }

		/// <summary>
		/// 窗口大小与位置
		/// </summary>
		public Rect windowRect { get; private set; }


		/// <summary>
		/// 唯一Id
		/// </summary>
		public int id { get; private set; }

		/// <summary>
		/// 用于计数的Id
		/// </summary>
		private static int hashId = 0;


		/// <summary>
		/// 创建新的窗口
		/// </summary>
		/// <param name="window"></param>
		/// <param name="rect"></param>
		/// <returns></returns>
		public static T Create(EditorWindow window, Rect rect)
		{
			T instance = Activator.CreateInstance<T>();
			instance.parent = window;
			instance.windowRect = rect;
			instance.id = hashId++;
			return instance;
		}

		protected BaseSubEditorWindow() { }

		/// <summary>
		/// 绘制
		/// </summary>
		/// <param name="window"></param>
		/// <param name="rect"></param>
		/// <returns></returns>
		public Rect Draw(EditorWindow window)
		{
			windowRect = GUILayout.Window(id, windowRect, DoWindow, "Hi There");
			return windowRect;
		}


		/// <summary>
		/// 绘制窗口
		/// </summary>
		/// <param name="unusedWindow"></param>
		protected abstract void DoWindow(int unusedWindow);
	}

}
