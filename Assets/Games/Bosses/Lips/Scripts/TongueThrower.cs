using Cysharp.Threading.Tasks;
using DG.Tweening;
using PL.Systems.Bosses.Lips;
using PL.Systems.Characters;
using PL.Systems.Grids;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Bosses.Tougues
{
    public class TongueThrower : MonoBehaviour
    {
        private Lip lip;
        private string attackTriggerName = "tongueAttack";
        private string openTriggerName = "openMouth";
        private string closeTriggerName = "closeMouth";
        private bool isAttacking;
        public int attackWidth = 4;
        public int attackHeight => GridMapManager.Instance.gridMap.height;

        public Animator lipAnimator;
        public Animator tongueAnimator;
        [Header("Attack")]
        public float toMoveDuration;
        public float baseHeight;
        public float waitDuration;
        public float releaseDuration;

        [Header("Sfx")]
        public AudioSource throwSfx;
        // Start is called before the first frame update
        void Start()
        {
            lip = GetComponent<Lip>();
            BossManager.Instance.leftHand.onGrapCharacterStart.AddListener(_ => OnGrapPlayer(true));
            BossManager.Instance.rightHand.onGrapCharacterStart.AddListener(_ => OnGrapPlayer(true));
            BossManager.Instance.leftHand.onAttackEnd.AddListener(() => OnGrapPlayer(false));
        }


        public void OnGrapPlayer(bool isGraped)
        {
            var leftX = 0;
            if (isGraped)
            {
                leftX = CharacterManager.Instance.character.X;
            }
            else
            {
                leftX = Random.Range(0, GridMapManager.Instance.gridMap.width - attackWidth + 1);
            }

            AttackAsync(leftX).AttachExternalCancellation(this.destroyCancellationToken).Forget();
        }

        [Button]
        public async UniTask AttackAsync(int leftX)
        {
            if (isAttacking)
            {
                return;
            }

            isAttacking = true;

            lip.StopMove();
            lipAnimator.SetTrigger(openTriggerName);
            var toPosition = new Vector3 (leftX + (attackWidth - 1) * 0.5f, baseHeight, transform.position.z);

            await transform.DOMove(toPosition, toMoveDuration);
            tongueAnimator.SetTrigger(attackTriggerName);
            throwSfx.Play();
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitDuration));
            TougueAttack(leftX);
            lipAnimator.SetTrigger(closeTriggerName);
            await UniTask.Delay(System.TimeSpan.FromSeconds(releaseDuration));
            lip.StartMove();

            isAttacking = false;
        }

        public void TougueAttack(int leftX)
        {
            var characterX = CharacterManager.Instance.character.X;
            var characterY = CharacterManager.Instance.character.Y;

            var attackGrids = GridMapManager.Instance.gridMap.GetEffectiveGrid(leftX, 0, attackWidth, attackHeight);

            if (attackGrids.Contains((characterX, characterY)))
            {
                //Debug.Log("Attack");

                CharacterManager.Instance.character.GetAttack();
            }
        }

    }

}
