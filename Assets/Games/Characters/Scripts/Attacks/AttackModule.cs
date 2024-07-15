using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace PL.Systems.Characters.Attacks
{
    public class AttackModule : MonoBehaviour
    {
        private int maxMarkerAmount = 3;
        //private bool isMarkAvailable = true;
        public List<GameObject> testDummies;
        public List<(int, int)> markers = new();
        public GameObject attackerPrefab;
        public float tileHeight = 0.01f;
        [Header("PrevInput")]
        public float prevInputTime;

        [Header("AttackCooldown")]
        public float coolDown = 7f;
        private float currentCoolDown;
        private bool isCoolDown;

        [Header("CooldownUI")]
        public Image frontImage;
        public TextMeshProUGUI uiText;

        [Header("Sfxs")]
        public AudioSource markerSfx;
        public AudioSource failSfx;
        private void Update()
        {
            if (!isCoolDown)
            {
                if (CharacterManager.Instance.character.moveController.moveState == Movements.CharacterMovementController.MoveState.Wait
    || CharacterManager.Instance.character.moveController.moveState == Movements.CharacterMovementController.MoveState.Stop)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        var x = CharacterManager.Instance.character.moveController.x;
                        var y = CharacterManager.Instance.character.moveController.y;

                        if (!markers.Contains((x, y)))
                        {
                            AddMarker(x, y);
                        }
                    }
                }
            }
            else
            {
                currentCoolDown -= Time.deltaTime;
                if (currentCoolDown <= 0f)
                {
                    currentCoolDown = 0f;
                    isCoolDown = false;
                }
            }

            UpdateUI();
        }

        public void AddMarker(int x, int y)
        {
            markerSfx.Play();
            markers.Add((x, y));
            testDummies[markers.Count - 1].transform.position = new Vector3(x, tileHeight, y);
            testDummies[markers.Count - 1].SetActive(true);
            if (markers.Count == maxMarkerAmount)
            {
                if (IsMarkAttackable())
                {
                    Attack();
                }
                else
                {
                    failSfx.Play();
                }
                Release();
            }
        }


        public bool IsMarkAttackable()
        {
            if (markers.Count != maxMarkerAmount)
            {
                return false;
            }

            var normalizedMarker = NormalizeMarker(markers);
            foreach ( var marker in normalizedMarker)
            {
                Debug.Log(marker);
            }
            if (!normalizedMarker[0].Equals((0, 0)))
            {
                return false;
            }

            if (!normalizedMarker[1].Equals((1, 1)))
            {
                return false;
            }

            if (!normalizedMarker[2].Equals((2, 0)))
            {
                return false;
            }

            return true;
        }

        public void Attack()
        {
            for ( int i = 0; i < markers.Count; i++ )
            {
                var attacker = Instantiate(attackerPrefab);
                attacker.transform.position = new Vector3(markers[i].Item1, 0f, markers[i].Item2);
                attacker.SetActive(true);
            }

            currentCoolDown = coolDown;
            isCoolDown = true;
            //Debug.Log("Attack");
        }

        public void Release()
        {
            markers.Clear();
            foreach (var item in testDummies)
            {
                item.SetActive(false);
            }
        }

        public List<(int, int)> NormalizeMarker(List<(int, int)> markers)
        {
            var minX = markers.Select(t => t.Item1).Min();
            var minY = markers.Select(t => t.Item2).Min();

            return markers.Select(t => (t.Item1 - minX, t.Item2 - minY)).OrderBy(t => t.Item1).ToList();
        }

        public void UpdateUI()
        {
            if (isCoolDown)
            {
                uiText.gameObject.SetActive(true);
                uiText.text = ((int)currentCoolDown).ToString();
                frontImage.fillAmount = 1f - (currentCoolDown / coolDown);
            }
            else
            {
                uiText.gameObject.SetActive(false);
                frontImage.fillAmount = 1f;
            }
            //public Image frontImage;
            //public TextMeshProUGUI uiText;
        }
    }

}
