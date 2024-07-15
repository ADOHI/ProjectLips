using Cysharp.Threading.Tasks;
using DG.Tweening;
using PL.Systems.Characters;
using PL.Systems.Grids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Bosses.Notes
{
    public class Note : MonoBehaviour
    {
        private int movePhase;
        private int x;
        private int y;
        private float destroyDistance = 50f;
        public List<GridMap.GridDirection> gridDirections;
        public int directionInt;
        public Transform spawnPoint;
        public float maxAngle;


        public (int, int) destination;

        public float minDuration;
        public float maxDuration;
        public float minHeightPower;
        public float maxHeightPower;

        public float speed;
        public float twistDuration;

        private void Update()
        {
            CheckToDestroy();
        }

        public async UniTask LandAsync(Vector3 fromPosition, Vector3 toDestination)
        {
            if (toDestination.x - fromPosition.x > 0f)
            {
                directionInt = 1;
            }
            else
            {
                directionInt = -1;
            }

            var heightPower = Random.Range(minHeightPower, maxHeightPower);
            var duration = Random.Range(minDuration, maxDuration);

            transform.DOMoveX(toDestination.x, duration).From(fromPosition.x);
            transform.DOMoveZ(toDestination.z, duration).From(fromPosition.z);
            await transform.DOMoveY(fromPosition.y + heightPower, duration * 0.5f).From(fromPosition.y);
            await transform.DOMoveY(toDestination.y, duration * 0.5f);
            this.x = (int)toDestination.x;
            this.y = (int)toDestination.z;
            MoveAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget(); ;
        }

        public async UniTask MoveAsync()
        {
            foreach (var direction in gridDirections)
            {
                switch (direction)
                {
                    case GridMap.GridDirection.Up:
                        await transform.DOMoveZ(transform.position.z + speed, twistDuration).WithCancellation(this.destroyCancellationToken);
                        this.y++;
                        break;
                    case GridMap.GridDirection.Down:
                        await transform.DOMoveZ(transform.position.z - speed, twistDuration).WithCancellation(this.destroyCancellationToken);
                        this.y--;
                        break;
                    case GridMap.GridDirection.Left:
                        await transform.DOMoveX(transform.position.x - speed * directionInt, twistDuration).WithCancellation(this.destroyCancellationToken);
                        this.x--;
                        break;
                    case GridMap.GridDirection.Right:
                        await transform.DOMoveX(transform.position.x + speed * directionInt, twistDuration).WithCancellation(this.destroyCancellationToken);
                        this.x++;
                        break;
                    case GridMap.GridDirection.None:
                        break;
                }
            }

            MoveAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();
        }

        public void CheckToDestroy()
        {
            if (Vector3.Distance(transform.position, GridMapManager.Instance.gridMap.center) > destroyDistance)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CharacterManager.Instance.character.GetAttack();

                Destroy(this.gameObject);
            }
        }
    }

}
