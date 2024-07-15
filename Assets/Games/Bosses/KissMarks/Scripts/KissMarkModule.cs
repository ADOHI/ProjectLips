using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PL.Systems.Characters;
using PL.Systems.Grids;
using PL.Systems.Ingames;
using QFSW.QC.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace PL.Systems.Bosses.KissMarks
{
    public class KissMarkModule : MonoBehaviour
    {
        [Serializable]
        public struct Difficulty
        {
            public float minCooltime;
            public float maxCooltime;
            public int amount;
            public float minInterval;
            public float maxInterval;
        }

        private CancellationTokenSource dropTokenSource = new();

        private Difficulty currentDifficulty;
        public Difficulty[] difficulties;

        public int kissMarkWidth = 4;
        public int kissMarkHeight = 3;

        public float kissMarkInitialHeight;
        public float kissMarkDropDuration;

        [Header("KissMarkSetting")]
        public SpriteRenderer kissMark;
        public float kissMarkSpawnHeight = 10f;

        [Header("KissMarkShadowSetting")]
        public SpriteRenderer kissMarkShadowPrefab;
        public float kissMarkGroundHeight = 0.1f;
        public float fromScale = 0.8f;
        public float toScale = 1f;
        public float fromAlpha = 0.8f;
        public float toAlpha = 1f;
        public float duration = 1f;
        public float navigatorDuration = 0.2f;
        public Ease dropEase;

        [Header("Audios")]
        public AudioSource attackStartSFX;
        public AudioSource markDropSFX;

        private void Start()
        {
            IngameManager.Instance.phase.ObserveEveryValueChanged(i => i.Value).Subscribe(ChangePhase);
            DropAsync().AttachExternalCancellation(dropTokenSource.Token);
        }

        [Button]
        public async UniTask DropOnceAsync()
        {
            var availableGrids = GridMapManager.Instance.gridMap.AvailableGrids(kissMarkWidth, kissMarkHeight);

            var randomCoordinate = availableGrids.RandomElement();

            var shadowSpawnPosition = new Vector3(randomCoordinate.Item1 + (kissMarkWidth - 1) * 0.5f, kissMarkGroundHeight, randomCoordinate.Item2 + (kissMarkHeight - 1) * 0.5f);

            var spawnedKissMarkShadow = Instantiate(kissMarkShadowPrefab);

            spawnedKissMarkShadow.transform.position = shadowSpawnPosition;
            spawnedKissMarkShadow.gameObject.SetActive(true);

            spawnedKissMarkShadow.transform.DOScale(kissMarkShadowPrefab.transform.localScale * toScale, duration).From(kissMarkShadowPrefab.transform.localScale * fromScale).SetEase(dropEase);
            spawnedKissMarkShadow.DOFade(toAlpha, 1f).From(fromAlpha).SetEase(dropEase);

            WarningNavigator.Instance.ShowNavigator(randomCoordinate.Item1, randomCoordinate.Item2, kissMarkWidth, kissMarkHeight, navigatorDuration);

            var spawnPosition = new Vector3(randomCoordinate.Item1 + (kissMarkWidth - 1) * 0.5f, kissMarkSpawnHeight, randomCoordinate.Item2 + (kissMarkHeight - 1) * 0.5f);

            var spawnedKissMark = Instantiate(kissMark);

            spawnedKissMark.transform.position = spawnPosition;
            spawnedKissMark.gameObject.SetActive(true);

            await spawnedKissMark.transform.DOMoveY(kissMarkGroundHeight, 1f).From(kissMarkSpawnHeight).OnComplete(() => KissMarkAttack(randomCoordinate.Item1, randomCoordinate.Item2)).SetEase(dropEase);
            //spawnedKissMarkShadow.DOFade(1f, 1f).From(0.8f);


            spawnedKissMarkShadow.DOFade(0f, 2f).OnComplete(() => Destroy(spawnedKissMarkShadow));
            spawnedKissMark.DOFade(0f, 2f).OnComplete(() => Destroy(spawnedKissMark));
        }

        public async UniTask DropAsync()
        {
            var cooltime = UnityEngine.Random.Range(currentDifficulty.minCooltime, currentDifficulty.maxCooltime);

            await UniTask.Delay(System.TimeSpan.FromSeconds(cooltime));
            attackStartSFX.Play();
            for (int i = 0; i < currentDifficulty.amount; i++)
            {
                DropOnceAsync().Forget();

                var interval = UnityEngine.Random.Range(currentDifficulty.minInterval, currentDifficulty.maxInterval);
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval));
            }

            DropAsync().AttachExternalCancellation(dropTokenSource.Token).Forget();
        }

        public void KissMarkAttack(int leftX, int downY)
        {
            var characterX = CharacterManager.Instance.character.X;
            var characterY = CharacterManager.Instance.character.Y;

            var attackGrids = GridMapManager.Instance.gridMap.GetEffectiveGrid(leftX, downY, kissMarkWidth, kissMarkHeight);

            if (attackGrids.Contains((characterX, characterY)))
            {
                //Debug.Log("Attack");

                CharacterManager.Instance.character.GetAttack();
            }
        }

        [Button]
        public void ChangePhase(int phase)
        {
            if (phase >= difficulties.Length)
            {
                dropTokenSource.Cancel();
                return;
            }
            currentDifficulty = difficulties[phase];

            //dropTokenSource = new CancellationTokenSource();

            //DropAsync().AttachExternalCancellation(dropTokenSource.Token);
        }

        private void OnDestroy()
        {
            //dropTokenSource.Cancel();
        }
    }

}
