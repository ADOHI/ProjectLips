using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NormalSceneManager : MonoBehaviour
{
    public float nextSceneMoveDuration = 5f;
    public float fadeDuration = 2f;
    public int nextSceneIndex = 0;
    public Image fadeImage;

    private void Start()
    {
        MoveSceneAsync();
    }

    private async UniTask MoveSceneAsync()
    {
        fadeImage.DOFade(0f, fadeDuration);
        await UniTask.Delay((int)(nextSceneMoveDuration * 1000f));
        SceneManager.LoadScene(0);
    }



}
