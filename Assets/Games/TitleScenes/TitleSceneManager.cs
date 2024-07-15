using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public List<Image> images;

    private void Start()
    {
        titleAsync();
    }

    public async UniTask titleAsync()
    {
        for (int i = 0; i < images.Count - 1; i++)
        {
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            await images[i].DOFade(0f, 0.2f);

        }

        await images[images.Count - 1].DOFade(1f, 2f);

        await SceneManager.LoadSceneAsync(1);
    }
}
