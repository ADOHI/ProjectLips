using DG.Tweening;
using MoreMountains.Feedbacks;
using PL.Systems.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Grids.MapBlocks
{
    public class MapBlock : MonoBehaviour
    {
        public int x;
        public int y;
        //public MMF_Player onCharacterEnter;
        //public MMF_Player onCharacterExit;

        [Header("Anim")]
        public float heightResponse = 0.05f;
        public float heightResponseDuration = 0.05f;
        public Ease ease;

        private void Start()
        {
            CharacterManager.Instance.character.moveController.onCharacterEnterGround.AddListener(OnCharacterEnter);
            CharacterManager.Instance.character.moveController.onCharacterExitGround.AddListener(OnCharacterExit);
        }

        public void OnCharacterEnter(int x, int y)
        {
            if (this.x == x && this.y == y)
            {
                transform.DOMoveY(-heightResponse, heightResponseDuration).SetEase(ease);
            }
        }

        public void OnCharacterExit(int x, int y)
        {
            if (this.x == x && this.y == y)
            {
                transform.DOMoveY(0f, heightResponseDuration).SetEase(ease);
            }
        }
    }

}
