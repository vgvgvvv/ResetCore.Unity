using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ResetCore.GameSystem
{
    public class Dialog
    {
        public int dialogId;
        public string title;
        public string context;
        public int nextId;
    }

    public class DialogManager
    {

        public static void ShowDialog(int id, Action<Dialog> showDialogAct)
        {
            showDialogAct(GetDialog(id));
        }

        private static Dialog GetDialog(int id)
        {
            return null;
        }

    }

}
