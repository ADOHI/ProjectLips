using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mono.CSharp.Linq;
using PL.Systems.Bosses.KissMarks;
using PL.Systems.Characters;
using PL.Systems.Grids;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static PL.Systems.Bosses.Hands.Hand;

namespace PL.Systems.Bosses.Hands
{
    public class Hand : MonoBehaviour
    {
        public enum HandType
        {
            Left,
            Right
        }

        public enum HandState
        {
            Idle,
            Move,
            Attack,
            Grap
        }

        private CancellationTokenSource idleTokenSource;
        private CancellationTokenSource attackTokenSource;
        private Animator animator;
        private HandState handState = HandState.Idle;
        private float attackZ;
        public HandType handType;
        public Vector3 basePosition;

        [Header("HeightPingPong")]
        public float pingpongDuration;
        public float pingpongHeight;
        public Ease ease;

        [Header("TickRotate")]
        public float maxTickAngle;
        public float minTickAngle;

        [Header("AttackSettings")]
        public List<(int, int)> attackMask;
        public float attackXPositionOffset;
        public float attackYPosition;
        private int attackZIndex;
        public string attackAnimStateName = "isAttack";
        public string grapAnimStateName = "isGrap";
        public string releaseAnimStateName = "isRelease";
        public int attackWidth = 5;
        public int attackHeight = 3;

        [Header("AttackTime")]
        public float releaseToScreen = 2f;
        public float moveToAttackReady = 1f;
        public float waitToAttack = 1f;
        public float moveAttack = 1f;
        public float waitAfterAttack = 1f;
        public float releaseAttack = 1f;
        public float comebackInitialPosition = 1f;

        [Header("Grap")]
        public Transform grapPoint;
        public float grapHeight;

        [Header("Events")]
        public UnityEvent<Transform> onGrapCharacterEnter;
        public UnityEvent<Transform> onGrapCharacterStart;
        public UnityEvent onAttackEnd;

        [Header("Sfxs")]
        public AudioSource grabSfx;
        public AudioSource moveSfx;
        private void Start()
        {
            animator = GetComponentInChildren<Animator>();

            if (handType == HandType.Right)
            {
                transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                basePosition = new Vector3(GridMapManager.Instance.width - basePosition.x, basePosition.y, basePosition.z);
            }
            transform.position = basePosition;


            idleTokenSource = new CancellationTokenSource();
            attackTokenSource = new CancellationTokenSource();
            MoveUpDownAsync().AttachExternalCancellation(idleTokenSource.Token).Forget();

            attackMask = GridMap.GenerateMask(attackWidth, attackHeight);

            CharacterManager.Instance.character.moveController.onReleaseGrap.AddListener(_ => CancelGrap());
        }

        private void Update()
        {
            IsAttackAvailable();
        }

        public async UniTask MoveUpDownAsync()
        {
            this.handState = HandState.Idle;
            transform.DOLocalMoveY(basePosition.y + pingpongHeight, pingpongDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ease).WithCancellation(idleTokenSource.Token);
        }

        [Button]
        public async UniTask AttackAsync(int z)
        {
            if (this.handState != HandState.Idle)
            {
                return;
            }

            //ttackZ = z;

            idleTokenSource.Cancel();

            this.handState = HandState.Attack;

            var attackStartXPos = (handType == HandType.Left) ? -attackXPositionOffset : GridMapManager.Instance.gridMap.width - 1 + attackXPositionOffset;

            var toAttackFirstPosition = new Vector3(attackStartXPos, attackYPosition, basePosition.z);




            await transform.DOMove(toAttackFirstPosition, releaseToScreen).WithCancellation(attackTokenSource.Token);

            //AttackMove

            var toAttackReadyPosition = new Vector3(attackStartXPos, attackYPosition, z);

            await transform.DOMove(toAttackReadyPosition, moveToAttackReady).WithCancellation(attackTokenSource.Token);

            //delay
            WarningNavigator.Instance.ShowNavigator(0, z, GridMapManager.Instance.width, this.attackHeight, waitToAttack);
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitToAttack));

            //Attack
            attackZIndex = z;
            animator.SetBool(attackAnimStateName, true);

            var attackEndXPos = (handType == HandType.Left) ? attackXPositionOffset + GridMapManager.Instance.gridMap.width - 1 : -attackXPositionOffset;

            var toAttackFinishPosition = new Vector3(attackEndXPos, attackYPosition, z);

            await transform.DOMove(toAttackFinishPosition, moveAttack).WithCancellation(attackTokenSource.Token);

            onAttackEnd.Invoke();

            //Release

            animator.SetBool(attackAnimStateName, false);
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitAfterAttack)).AttachExternalCancellation(attackTokenSource.Token);

            var afterAttackPosition = new Vector3(attackEndXPos, attackYPosition, basePosition.z);


            await transform.DOMove(afterAttackPosition, releaseAttack).WithCancellation(attackTokenSource.Token);

            await transform.DOMove(basePosition, comebackInitialPosition).WithCancellation(attackTokenSource.Token);

            idleTokenSource = new CancellationTokenSource();
            MoveUpDownAsync().AttachExternalCancellation(idleTokenSource.Token).Forget();
        }

        private (int, int) GetCurrentGrid()
        {
            var gridX = (int)transform.position.x - (attackWidth / 2);
            var gridY = attackZIndex - (attackHeight / 2);

            return (gridX, gridY);
        }

        public List<(int, int)> GetCurrentGrids()
        {
            var (gridX, gridY) = GetCurrentGrid();

            return GridMapManager.Instance.gridMap.GetEffectiveGrid(gridX, gridY, attackMask);
        }

        public void IsAttackAvailable()
        {
            if (handState != HandState.Attack)
            {
                return;
            }

            var currentGrid = GetCurrentGrids();

            var characterX = CharacterManager.Instance.character.X;
            var characterY = CharacterManager.Instance.character.Y;

/*            if (currentGrid.Contains((characterX, characterY)))
            {
                Debug.Log((characterX, characterY, GetCurrentGrid()));
                GrapAsync();
            }
*/
            var attackGrids = GridMapManager.Instance.gridMap.GetEffectiveGrid((int)transform.position.x - (attackWidth / 2), attackZIndex, attackWidth, attackHeight);

            Debug.Log((attackZIndex, characterX, characterY, attackGrids.Contains((characterX, characterY))));
            foreach ( var attackGrid in attackGrids )
            {
                Debug.Log(attackGrid);
            }
            if (attackGrids.Contains((characterX, characterY)))
            {
                Debug.Log((characterX, characterY, GetCurrentGrid()));
                GrapAsync();
            }
        }

        public async UniTask GrapAsync()
        {
            //attach to
            attackTokenSource.Cancel();
            handState = HandState.Grap;
            onGrapCharacterEnter.Invoke(this.grapPoint);
            animator.SetBool(grapAnimStateName, true);
            var betweenTo = CharacterManager.Instance.character.transform.position - grapPoint.position;
            await transform.DOMove(transform.position + betweenTo, 1f);
            animator.SetBool(attackAnimStateName, false);
            onGrapCharacterStart.Invoke(this.grapPoint);
            grabSfx.Play();
            //up to
            await transform.DOMove(transform.position + Vector3.up * grapHeight, 1f);
        }

        public void CancelGrap()
        {
            if (handState != HandState.Grap)
            {
                return;
            }
            ReleaseGrap().Forget();
        }

        public async UniTask ReleaseGrap()
        {
            animator.SetBool(grapAnimStateName, false);
            animator.SetBool(attackAnimStateName, false);
            animator.SetBool(releaseAnimStateName, true);

            await UniTask.Delay(1000);

            this.handState = HandState.Idle;

            animator.SetBool(releaseAnimStateName, false);

            var attackEndXPos = (handType == HandType.Left) ? attackXPositionOffset + GridMapManager.Instance.gridMap.width - 1 : -attackXPositionOffset;

            var toAttackFinishPosition = new Vector3(attackEndXPos, attackYPosition, transform.position.z);

            //var toAttackFinishPosition = new Vector3(attackXPositionOffset + GridMapManager.Instance.gridMap.width - 1, attackYPosition, transform.position.z);

            await transform.DOMove(toAttackFinishPosition, moveAttack).WithCancellation(this.destroyCancellationToken);

            await UniTask.Delay(System.TimeSpan.FromSeconds(waitAfterAttack)).AttachExternalCancellation(this.destroyCancellationToken);


            var afterAttackPosition = new Vector3(attackEndXPos, attackYPosition, basePosition.z);


            //var afterAttackPosition = new Vector3(attackXPositionOffset + GridMapManager.Instance.gridMap.width - 1, attackYPosition, basePosition.z);

            await transform.DOMove(afterAttackPosition, releaseAttack).WithCancellation(this.destroyCancellationToken);

            await transform.DOMove(basePosition, comebackInitialPosition).WithCancellation(this.destroyCancellationToken);

            idleTokenSource = new CancellationTokenSource();
            attackTokenSource = new CancellationTokenSource();

            MoveUpDownAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();
        }

        [Button]
        public void Cancel()
        {
            idleTokenSource.Cancel();
        }

        public void OnHandOnGround()
        {
            moveSfx.Play();
        }

    }

}
