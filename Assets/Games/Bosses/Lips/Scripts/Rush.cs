using Cysharp.Threading.Tasks;
using DG.Tweening;
using PL.Systems.Bosses;
using PL.Systems.Bosses.KissMarks;
using PL.Systems.Bosses.Lips;
using PL.Systems.Characters;
using PL.Systems.Grids;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Ingames
{
    public class Rush : MonoBehaviour
    {
        private float initialZ;
        private Lip lip;
        private string openTriggerName = "openMouth";
        private string closeTriggerName = "closeMouth";
        private bool isAttacking;
        public Animator lipAnimator;
        [Header("Attack")]
        public int attackHeight = 5;
        public float toMoveDuration;
        public float toPositionZ = 1.5f;
        public float baseHeight;
        public float toScale;
        public float waitDuration;
        public float rushDuration;
        public float releaseDuration;

        public Ease rushEase;


        [Header("SFXs")]
        public AudioSource rushSfx;

        public int attackWidth => GridMapManager.Instance.width;

        // Start is called before the first frame update
        void Start()
        {
            lip = GetComponent<Lip>();
            initialZ = transform.position.z;
        }

        [Button]
        public async UniTask AttackAsync()
        {
            if (isAttacking)
            {
                return;
            }

            isAttacking = true;

            lip.StopMove();
            var centerX = GridMapManager.Instance.width / 2;
            lipAnimator.SetTrigger(openTriggerName);
            var toPosition = new Vector3 (centerX, baseHeight, transform.position.z);
            await transform.DOMove(toPosition, toMoveDuration);

            WarningNavigator.Instance.ShowNavigator(0, GridMapManager.Instance.height - attackHeight, attackWidth, attackHeight, waitDuration);

            await UniTask.Delay(System.TimeSpan.FromSeconds(waitDuration));
            //rush
            var RushPosition = new Vector3 (centerX, baseHeight, toPositionZ);
            rushSfx.Play();
            transform.DOMove(RushPosition, rushDuration).SetEase(rushEase);
            await transform.DOScale(toScale, rushDuration).SetEase(rushEase);
            RushAttack();
            //wait
            await UniTask.Delay(System.TimeSpan.FromSeconds(releaseDuration));
            //Release

            lipAnimator.SetTrigger(closeTriggerName);
            transform.DOMove(toPosition, rushDuration).SetEase(rushEase);
            await transform.DOScale(1f, rushDuration).SetEase(rushEase);

            lip.StartMove();

            isAttacking = false;
        }

        public void RushAttack()
        {
            var characterX = CharacterManager.Instance.character.X;
            var characterY = CharacterManager.Instance.character.Y;

            var attackGrids = GridMapManager.Instance.gridMap.GetEffectiveGrid(0, GridMapManager.Instance.height - attackHeight, attackWidth, attackHeight);

            if (attackGrids.Contains((characterX, characterY)))
            {
                CharacterManager.Instance.character.GetAttack();
            }
        }
    }

}
