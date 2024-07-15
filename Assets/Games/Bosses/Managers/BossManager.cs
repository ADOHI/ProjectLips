using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using Pixelplacement;
using PL.Systems.Bosses.Hands;
using PL.Systems.Bosses.KissMarks;
using PL.Systems.Bosses.Lips;
using PL.Systems.Ingames;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PL.Systems.Bosses
{
    public class BossManager : Singleton<BossManager>
    {
        [Serializable]
        public struct ShakeDifficulty
        {
            public float minCooltime;
            public float maxCooltime;
            public float minDuration;
            public float maxDuration;
        }


        private int currentPhase => IngameManager.Instance.phase;
        private ShakeDifficulty currentShakeDifficulty;

        public ShakeDifficulty[] shakeDifficulties;

        public Lip lip;
        public KissMarkModule kissMarkModule;
        public Rush rush;
        [Header("Stats")]
        public IntReference[] maxHealths;
        public IntReference[] currentHealths;

        [Header("Hand")]
        private int leftHandZ = 1;
        private int rightHandZ = 4;
        public Hand leftHand;
        public Hand rightHand;
        public float handMinCooltime;
        public float handMaxCooltime;

        [Header("ShakeSetting")]
        public int minShakePhase = 1;
        //public float shakeDuration;
        public MMF_Player shakeFeedback;
        public UnityEvent onBoardShakeStart;
        public UnityEvent onBoardShakeEnd;

        [Header("Sfx")]
        public AudioSource hitSfx;
        private void Awake()
        {
            currentHealths[0].Value = maxHealths[0].Value;
            currentHealths[1].Value = maxHealths[1].Value;
            currentHealths[2].Value = maxHealths[2].Value;
        }

        private void Start()
        {
            IngameManager.Instance.phase.ObserveEveryValueChanged(i => i.Value).Subscribe(ChangePhase);
        }



        [Button]
        public async UniTask ShakeBoardOnceAsync(float duration)
        {
            shakeFeedback.PlayFeedbacks();
            onBoardShakeStart.Invoke();
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
            onBoardShakeEnd.Invoke();
        }

        public async UniTask ShakeBoardAsync()
        {
            var cooltime = UnityEngine.Random.Range(currentShakeDifficulty.minCooltime, currentShakeDifficulty.maxCooltime);

            await UniTask.Delay(System.TimeSpan.FromSeconds(cooltime));

            var duration = UnityEngine.Random.Range(currentShakeDifficulty.minDuration, currentShakeDifficulty.maxDuration);

            ShakeBoardOnceAsync(duration);
          
            if (IngameManager.Instance.phase.Value != 2)
            {
                ShakeBoardAsync().AttachExternalCancellation(this.destroyCancellationToken);
            }
            else
            {
                //Rush
                await rush.AttackAsync();

                if (LucidRandom.valueBool)
                {
                    ShakeBoardAsync().AttachExternalCancellation(this.destroyCancellationToken);
                }
                else
                {
                    AttackHand().AttachExternalCancellation(this.destroyCancellationToken);
                }
            }
        }

        [Button]
        public async UniTask AttackHand()
        {
            var cooltime = UnityEngine.Random.Range(handMinCooltime, handMaxCooltime);

            await UniTask.Delay(System.TimeSpan.FromSeconds(cooltime));
            leftHand.AttackAsync(leftHandZ).Forget();
            rightHand.AttackAsync(rightHandZ);

            if (LucidRandom.valueBool)
            {
                ShakeBoardAsync().AttachExternalCancellation(this.destroyCancellationToken);
            }
            else
            {
                AttackHand().AttachExternalCancellation(this.destroyCancellationToken);
            }
        }



        public void ChangePhase(int phase)
        {
            if (phase < minShakePhase)
            {
                return;
            }

            if (phase == minShakePhase)
            {
                ShakeBoardAsync();
            }

            currentShakeDifficulty = shakeDifficulties[phase - minShakePhase];
        }

        [Button]
        public void GetDamage()
        {
            hitSfx.Play();
            currentHealths[currentPhase].Value--;

            if (currentHealths[currentPhase].Value <= 0)
            {
                if (IngameManager.Instance.phase.Value == 2)
                {
                    SceneManager.LoadScene(3);
                }


                IngameManager.Instance.IncreasePhase();

                
            }
        }


    }

}

