using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mono.CSharp;
using MoreMountains.Tools;
using PL.Systems.Bosses;
using PL.Systems.Grids;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static PL.Systems.Characters.Movements.CharacterMovementController;

namespace PL.Systems.Characters.Movements
{
    public class CharacterMovementController : MonoBehaviour
    {
        public enum MoveState
        {
            Stop,
            Wait,
            Move,
            Grap,
            Release
        }

        private GridMap gridMap => GridMapManager.Instance.gridMap;
        private bool isMoveAvailable = true;
        private GridMap.GridDirection currentDirection = GridMap.GridDirection.Down;
        [HideInInspector] public int x, y;
        public MoveState moveState = MoveState.Stop;
        private bool isStunEffected = false;
        private Transform grapTransform;
        private CancellationTokenSource moveTokenSource;

        [Header("Move")]
        public float moveDeley = 1f;
        public float moveDuration = 0.8f;
        public Ease upXEase;
        public Ease upYEase;
        public Ease downXEase;
        public Ease downYEase;

        [Header("Grap")]
        private float grapResistance = 0f;
        public float grapReleaseOnHit = 0.1f;
       

        [Header("Ground")]
        public LayerMask groundMask;

        [Header("Events")]
        public UnityEvent<int, int> onCharacterEnterGround;
        public UnityEvent<int, int> onCharacterExitGround;
        public UnityEvent<GridMap.GridDirection, GridMap.GridDirection> onCharacterMoveStart;
        public UnityEvent<GridMap.GridDirection> onReleaseGrap;
        private Sequence moveSequence;

        [Header("Sfxs")]
        public AudioSource moveSfx;
        public AudioSource grapedSfx;

        [Header("UIs")]
        public GameObject ui;
        void Start()
        {
            x = gridMap.centerWidth;
            y = gridMap.centerHeight;
            onCharacterEnterGround.Invoke(x, y);
            SetPosition();

            BossManager.Instance.onBoardShakeStart.AddListener(()=> isStunEffected = true);
            BossManager.Instance.onBoardShakeEnd.AddListener(()=> isStunEffected = false);

            BossManager.Instance.leftHand.onGrapCharacterEnter.AddListener(Grap);
            BossManager.Instance.rightHand.onGrapCharacterEnter.AddListener(Grap);
        }

        // Update is called once per frame
        void Update()
        {
            switch (moveState)
            {
                case MoveState.Stop:
                    Move();
                    AttachGround();
                    break;
                case MoveState.Wait:
                    AttachGround();
                    break;
                case MoveState.Move:
                    break;
                case MoveState.Grap:
                    AttachGrap();
                    ReleaseGrap();
                    break;
            }

            ui.SetActive(isStunEffected);
        }

        public void Move()
        {
            if (!isMoveAvailable)
            {
                return;
            }

            var nextX = x;
            var nextY = y;

            var nextDirection = GridMap.GridDirection.None;


            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (isStunEffected)
                {
                    nextDirection = GridMap.GridDirection.Down;
                }
                else nextDirection = GridMap.GridDirection.Up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (isStunEffected)
                {
                    nextDirection = GridMap.GridDirection.Up;
                }
                else nextDirection = GridMap.GridDirection.Down;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (isStunEffected)
                {
                    nextDirection = GridMap.GridDirection.Right;
                }
                else nextDirection = GridMap.GridDirection.Left;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (isStunEffected)
                {
                    nextDirection = GridMap.GridDirection.Left;
                }
                else nextDirection = GridMap.GridDirection.Right;
            }

            moveTokenSource = new CancellationTokenSource();
            MoveAsync(nextDirection).AttachExternalCancellation(this.moveTokenSource.Token).Forget();

        }

        public async UniTask MoveAsync(GridMap.GridDirection direction)
        {
            var nextX = x;
            var nextY = y;

            switch (direction)
            {
                case GridMap.GridDirection.Up:
                    nextY++;
                    break;
                case GridMap.GridDirection.Down:
                    nextY--;
                    break;
                case GridMap.GridDirection.Left:
                    nextX--;
                    break;
                case GridMap.GridDirection.Right:
                    nextX++;
                    break;
                case GridMap.GridDirection.None:
                    break;
            }

            if (!gridMap.IsValidTile(nextX, nextY))
            {
                return;
            }

            if (!IsMoveAvailable(x, y, nextX, nextY))
            {
                return;
                //x = nextX;
                //y = nextY;
                //SetPosition();
            }

            moveState = MoveState.Move;
            isMoveAvailable = false;

            onCharacterExitGround.Invoke(x, y);
            onCharacterMoveStart.Invoke(currentDirection, direction);
            currentDirection = direction;

            moveSfx.Play();

            var toCenterPosition = new Vector3((x + nextX) * 0.5f, 1f, (y + nextY) * 0.5f);

            moveSequence = DOTween.Sequence();

            await moveSequence.Join(transform.DOMoveX(toCenterPosition.x, moveDuration * 0.5f).SetEase(upXEase))
                .Join(transform.DOMoveY(toCenterPosition.y, moveDuration * 0.5f).SetEase(upYEase))
                .Join(transform.DOMoveZ(toCenterPosition.z, moveDuration * 0.5f).SetEase(upXEase))
                .WithCancellation(moveTokenSource.Token);

            x = nextX;
            y = nextY;

            var toPosition = new Vector3(x, 0f, y);

            moveSequence = DOTween.Sequence();

            await moveSequence.Join(transform.DOMoveX(toPosition.x, moveDuration * 0.5f).SetEase(downXEase))
                .Join(transform.DOMoveY(toPosition.y, moveDuration * 0.5f).SetEase(downXEase))
                .Join(transform.DOMoveZ(toPosition.z, moveDuration * 0.5f).SetEase(downXEase))
                .WithCancellation(moveTokenSource.Token);

            await UniTask.Delay(50).AttachExternalCancellation(moveTokenSource.Token);

            moveState = MoveState.Wait;

            onCharacterEnterGround.Invoke(x, y);

            await UniTask.Delay(System.TimeSpan.FromSeconds(moveDeley - moveDuration)).AttachExternalCancellation(moveTokenSource.Token);

            ReleaseMove();
            //isMoveAvailable = true;
        }

        private void SetPosition()
        {
            transform.position = new Vector3(x, 0f, y);
        }

        private void ReleaseMove()
        {
            isMoveAvailable = true;
            moveState = MoveState.Stop;
        }

        private bool IsMoveAvailable(int currentX, int currentY, int nextX, int nextY)
        {
            if (!gridMap.tiles[nextX, nextY].valid)
            {
                return false;
            }

            if (gridMap.IsPassAvailable(currentX, currentY, nextX, nextY))
            {
                return true;
            }

            return false;
        }

        public void AttachGround()
        {
            var ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out var hitInfo, 100f, groundMask))
            {
                transform.position = transform.position.MMSetY(hitInfo.point.y);
            }


        }

        public void Grap(Transform grapTransform)
        {
            moveTokenSource.Cancel();
            moveSequence.Kill();
            this.moveState = MoveState.Grap;
            Debug.Log("Grap");

            this.grapTransform = grapTransform;
        }

        public void AttachGrap()
        {
            transform.position = grapTransform.position;

            grapedSfx.Play();
        }

        public void ReleaseGrap()
        {
            if (this.moveState != MoveState.Grap)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                grapResistance += grapReleaseOnHit;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                grapResistance += grapReleaseOnHit;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                grapResistance += grapReleaseOnHit;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                grapResistance += grapReleaseOnHit;
            }

            Debug.Log(grapResistance);

            if (grapResistance > 1f)
            {
                grapResistance = 0f;
                onReleaseGrap.Invoke(currentDirection);
                ReleaseGrapAsync().Forget();

                grapedSfx.Stop();
            }
        }

        public async UniTask ReleaseGrapAsync()
        {
            moveState = MoveState.Release;
            await transform.DOMove(new Vector3(x, 0f, y), 5f).SetSpeedBased();
            ReleaseMove();
        }

        [Button]
        public void EffectStatus(float duration)
        {
            isStunEffected = true;
            Invoke("ReleaseStatusEffect", duration);
        }

        private void ReleaseStatusEffect()
        {
            isStunEffected = false;
        }
    }

}
