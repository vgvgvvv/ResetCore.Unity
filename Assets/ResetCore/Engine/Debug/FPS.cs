using UnityEngine;
using System.Collections;
using ResetCore.Util;

namespace ResetCore.Util.Debugger
{
    public class FPS : MonoSingleton<FPS>
    {
        private int frames = 0;
        private float updateInterval = 0.5f;
        private float fps;
        private float accum = 0.0f;
        private float timeLeft;
        public string version;

        void Start()
        {
            timeLeft = updateInterval;
        }

         void Update()
        {
            timeLeft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
           ++frames;
            if (timeLeft <= 0.0f)
            {
                fps = (accum / frames);//.ToString("f2");
                timeLeft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }
        }

        void OnGUI()
        {
            GUIStyle bb = new GUIStyle();
            bb.normal.background = null;    //这是设置背景填充的
            bb.normal.textColor = new Color(1, 0, 0);   //设置字体颜色的
            bb.fontSize = 40;       //当然，这是字体颜色

            //Color cq = GUI.color;
            GUI.color = Color.red;
            //GUI.Label(new Rect(Screen.width / 2, 0, 200, 200), MogoWorld.theGdata.strText, bb);
            GUI.Label(new Rect(Screen.width / 2, 0, 200, 200), "FPS: " + fps, bb);

            //GUI.Label(new Rect(0, 30, 200, 200), "memory： " + ConvertBytesToMegebytes(Profiler.usedHeapSize)
            //    + "  :  "+ConvertBytesToMegebytes(System.GC.GetTotalMemory(true)), bb);

        }

        static double ConvertBytesToMegebytes(long bytes)
        {
           return (bytes / 1024f) / 1024f;
        }
    }

}
