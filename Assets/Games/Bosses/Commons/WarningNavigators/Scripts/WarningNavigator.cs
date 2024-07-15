using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Bosses.KissMarks
{
    public class WarningNavigator : Singleton<WarningNavigator>
    {
        public SpriteRenderer navigatorPrefab;
        public Color fromColor;
        public Color toColor;

        public async UniTask ShowNavigator(int leftX, int downY, int width, int height, float duration)
        {
            var spawnedNavigator = Instantiate(navigatorPrefab);

            var centerX = leftX + (width - 1f) * 0.5f;
            var centerY = downY + (height - 1f) * 0.5f;

            spawnedNavigator.transform.position = new Vector3(centerX, 0.1f, centerY);

            spawnedNavigator.size = new Vector3(width, height);

            spawnedNavigator.gameObject.SetActive(true);

            await spawnedNavigator.DOColor(toColor, duration).From(fromColor);



            Destroy(spawnedNavigator);
        }
    }

}
