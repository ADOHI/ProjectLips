using Cysharp.Threading.Tasks;
using DG.Tweening;
using PL.Systems.Grids;
using PL.Systems.Ingames;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace PL.Systems.Bosses.Lips
{
    public class Lip : MonoBehaviour
    {
        private CancellationTokenSource moveTokenSource;
        [HideInInspector] public NoteThrower noteThrower;
        public int x => Mathf.RoundToInt(transform.position.x);


        private float leftX = 0;
        private float rightX => GridMapManager.Instance.width - 1;

        public GameObject[] lipModels;

        public float maxY;
        public float minY;

        public float moveSpeedX;
        public float moveSpeedY;

        [Header("Status")]
        public IntReference maxHealth;
        public IntReference currentHealth;

        private void Start()
        {
            noteThrower = GetComponent<NoteThrower>();
            StartMove();

            IngameManager.Instance.phase.ObserveEveryValueChanged(v => v.Value).Subscribe(OnNextPhase);
        }

        public async UniTask MoveXAsync()
        {
            await transform.DOMoveX(leftX, moveSpeedX).SetSpeedBased().WithCancellation(moveTokenSource.Token);
            await transform.DOMoveX(rightX, moveSpeedX).SetSpeedBased().WithCancellation(moveTokenSource.Token);

            MoveXAsync().AttachExternalCancellation(moveTokenSource.Token).Forget();
        }

        public async UniTask MoveYAsync()
        {
            await transform.DOMoveY(minY, moveSpeedY).SetSpeedBased().WithCancellation(moveTokenSource.Token);
            await transform.DOMoveY(maxY, moveSpeedY).SetSpeedBased().WithCancellation(moveTokenSource.Token);

            MoveYAsync().AttachExternalCancellation(moveTokenSource.Token).Forget();
        }

        public void StartMove()
        {
            moveTokenSource = new CancellationTokenSource();

            MoveXAsync().AttachExternalCancellation(moveTokenSource.Token).Forget();
            MoveYAsync().AttachExternalCancellation(moveTokenSource.Token).Forget();
        }

        public void StopMove()
        {
            moveTokenSource.Cancel();
        }

        public void OnNextPhase(int phase)
        {
            if (phase == 0)
            {
                return;
            }
            lipModels[phase].SetActive(true);
            lipModels[phase - 1].SetActive(false);
        }


    }

}
