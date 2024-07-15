using PL.Systems.Bosses;
using PL.Systems.Ingames;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace PL.Systems.UIs
{
    public class EnemyHealthBar : MonoBehaviour
    {
        private List<Image> healthBarImages = new();
        public float barSize = 480f;
        public IntReference maxHealth => BossManager.Instance.maxHealths[IngameManager.Instance.phase];
        public IntReference currentHealth => BossManager.Instance.currentHealths[IngameManager.Instance.phase];

        public Image healthImagePrefab;
        public float size;

        private void Awake()
        {
            //Spawn();
        }

        private void Start()
        {
            IngameManager.Instance.phase.ObserveEveryValueChanged(i => i.Value)
                .Subscribe(_ =>
                {
                    Spawn();
                    currentHealth.ObserveEveryValueChanged(i => i.Value).Subscribe(UpdateHealth);
                });
        }

        private void Spawn()
        {
            healthBarImages.ForEach(i => Destroy(i.gameObject));
            healthBarImages.Clear();
            for (int i = 0; i < maxHealth.Value; i++)
            {
                var healthBarImage = Instantiate(healthImagePrefab);

                healthBarImage.transform.SetParent(transform, false);

                healthBarImage.rectTransform.sizeDelta = new Vector2(barSize / maxHealth.Value, healthImagePrefab.rectTransform.sizeDelta.y);

                healthBarImage.gameObject.SetActive(true);

                healthBarImages.Add(healthBarImage);
            }
        }

        private void UpdateHealth(int currentHealth)
        {
            if (currentHealth > maxHealth)
            {
                return;
            }

            for (int i = 0; i < currentHealth; i++)
            {
                healthBarImages[i].color = new Color(1f, 1f, 1f, 1f);
            }
            for (int i = currentHealth; i < maxHealth.Value; i++)
            {
                healthBarImages[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

}

