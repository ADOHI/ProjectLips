using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pixelplacement;
using QFSW.QC;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PL.Systems.UIs.Dialogs
{
    public class DialogManager : MonoBehaviour
    {
        [Serializable]
        public struct DialogData
        {
            public int speakerIndex;
            public string speakerName;
            public string dialog;
            public Color color;
        }

        public Canvas canvas;
        public Image fadeImage;
        public TextMeshProUGUI subjectText;
        public TextMeshProUGUI dialogText;
        public AudioSource[] dialogAudios;

        public float dialogSpeed;
        public float dialogAutoSkipDuration;

        public DialogData[] data;

        public Animation animation;

        [Button]
        public async UniTask ShowDialogOnceAsync(DialogData dialog)
        {


            fadeImage.gameObject.SetActive(dialog.speakerIndex == 1);

            if (dialog.speakerIndex == 0)
            {
                animation.Play();
            }
            //dialogAudios[dialog.speakerIndex].Play();
            subjectText.color = dialog.color;
            dialogText.color = dialog.color;
            subjectText.text = dialog.speakerName;
            await dialogText.DOText(dialog.dialog, dialogSpeed).SetSpeedBased();
            dialogAudios[dialog.speakerIndex].Stop();
            await UniTask.Delay(System.TimeSpan.FromSeconds(dialogAutoSkipDuration));
        }

        [Button]
        public async UniTask ShowDialogAsync()
        {
            foreach (var dialog in data)
            {
                await ShowDialogOnceAsync(dialog);
            }
        }
    }
}




