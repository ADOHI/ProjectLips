using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Threading.Tasks;
using PL.Systems.Bosses.Notes;
using PL.Systems.Grids;
using PL.Systems.Ingames;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PL.Systems.Bosses.Lips
{
    public class NoteThrower : MonoBehaviour
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

        private Lip lip;

        private Difficulty currentDifficulty;
        public Difficulty[] difficulties;

        public List<Note> notePrefabs;
        public Transform spawnPoint;

        public float noteHeight = 0f;


        [Header("NoteSpread")]
        public int noteSpreadRange;

        [Header("SFXs")]
        public AudioSource singSfx;

        private void Awake()
        {
            lip = GetComponent<Lip>();
        }

        private void Start()
        {
            IngameManager.Instance.phase.ObserveEveryValueChanged(i => i.Value).Subscribe(ChangePhase);
            ThrowAsync().AttachExternalCancellation(this.destroyCancellationToken).Forget();
        }

        public async UniTask ThrowAsync()
        {
            var cooltime = UnityEngine.Random.Range(currentDifficulty.minCooltime, currentDifficulty.maxCooltime);

            await UniTask.Delay(System.TimeSpan.FromSeconds(cooltime));
            singSfx.Play();
            for (int i = 0; i < currentDifficulty.amount; i++)
            {
                FireNote();

                var interval = UnityEngine.Random.Range(currentDifficulty.minInterval, currentDifficulty.maxInterval);
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval));
            }

            ThrowAsync().AttachExternalCancellation(this.destroyCancellationToken);
        }

        [Button]
        public void FireNote()
        {
            var destX = UnityEngine.Random.Range(Mathf.Max(0, lip.x - noteSpreadRange), Mathf.Min(lip.x + noteSpreadRange, GridMapManager.Instance.gridMap.width - 1));
            var randomIndex = UnityEngine.Random.Range(IngameManager.Instance.phase.Value * 3, IngameManager.Instance.phase.Value * 3 + 3);
            var spawnedNote = Instantiate(notePrefabs[randomIndex]);
            spawnedNote.spawnPoint = spawnPoint;
            spawnedNote.gameObject.SetActive(true);
            spawnedNote.LandAsync(spawnPoint.position, new Vector3(destX, noteHeight, GridMapManager.Instance.gridMap.height - 1))
                .AttachExternalCancellation(spawnedNote.destroyCancellationToken);
        }

        public void ChangePhase(int phase)
        {
            if (phase > difficulties.Length)
            {
                return;
            }
            currentDifficulty = difficulties[phase];
        }
    }
}
