using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace PL.Systems.UIs
{
    public class HealthBar : MonoBehaviour
    {
        private List<Image> healthBarImages = new();
        public IntReference maxHealth;
        public IntReference currentHealth;

        public Image healthImagePrefab;
        public float size;
        private void Awake()
        {
            Spawn();
        }

        private void Start()
        {
            currentHealth.ObserveEveryValueChanged(i => i.Value).Subscribe(UpdateHealth);
        }

        private void Spawn()
        {
            for (int i = 0; i < maxHealth; i++)
            {
                var healthBarImage = Instantiate(healthImagePrefab);

                healthBarImage.transform.SetParent(transform, false);

                healthBarImage.rectTransform.sizeDelta = Vector2.one * size;

                healthBarImage.gameObject.SetActive(true);

                healthBarImages.Add(healthBarImage);
            }
        }

        private void UpdateHealth(int currentHealth)
        {
            if (currentHealth < 0)
            {
                return;
            }

            if (currentHealth < maxHealth)
            {
                for (int i = currentHealth; i < maxHealth; i++)
                {
                    healthBarImages[i].DOFade(0f, 0.2f);
                }
            }
        }
    }

}
