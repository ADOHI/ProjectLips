using Ami.BroAudio.Demo;
using PL.Systems.Characters.Movements;
using PL.Systems.Characters.Sprites;
using PL.Systems.Ingames;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace PL.Systems.Characters
{
    public class PlayableCharacter : MonoBehaviour
    {
        public int X => moveController.x;
        public int Y => moveController.y;

        public CharacterMovementController moveController;
        public CharacterSpriteSwapper spriteSwaper;

        public IntReference maxHealth;
        public IntReference currentHealth;


        [Header("Sfxs")]
        public AudioSource getHitSfx;
        private void Awake()
        {
            moveController = GetComponent<CharacterMovementController>();
            spriteSwaper = GetComponent<CharacterSpriteSwapper>();

            currentHealth.Value = maxHealth.Value;
        }

        public void GetAttack()
        {
            getHitSfx.Play();
            currentHealth.Value--;

            if (currentHealth.Value <= 0 )
            {
                IngameManager.Instance.EndGame();
            }
        }

    }

}
