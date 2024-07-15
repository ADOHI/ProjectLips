using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PL.Systems.UIs.Dialogs.DialogManager;

namespace PL.Systems.UIs.Dialogs
{
    [CreateAssetMenu(fileName = "DialogData", menuName = "ScriptableObjects/Dialog", order = 1)]
    public class DialogChunk : ScriptableObject
    {
        public DialogData[] data;
    }

}
