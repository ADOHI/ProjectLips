using Cysharp.Threading.Tasks;
using DG.Tweening;
using PL.Systems.Ingames;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    public Image fadeImage;
    public Image[] images;
    public float fadeDuration = 2f;
    private void Start()
    {
        IngameManager.Instance.phase.ObserveEveryValueChanged(x => x.Value).Subscribe(Show);
    }

    public void Show(int phase)
    {
        ShowPrologue(phase).Forget();
    }
    public async UniTask ShowPrologue(int phase)
    {
        if (phase == 0)
        {
            return;
        }
        Time.timeScale = 0f;
        Debug.Log(phase);
        images[phase - 1].gameObject.SetActive(true);

        await images[phase - 1].DOFade(1f, fadeDuration).From(0f).SetUpdate(true);
        await UniTask.Delay(2000, ignoreTimeScale: true);
        await images[phase - 1].DOFade(0f, fadeDuration).SetUpdate(true);

        images[phase - 1].gameObject.SetActive(false);
        Time.timeScale = 1f;

    }

}
