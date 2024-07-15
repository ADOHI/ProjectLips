using DG.Tweening;
using PL.Systems.Bosses;
using PL.Systems.Grids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Characters.Attacks
{
    public class Attacker : MonoBehaviour
    {
        private float destroyDistance = 50f;


        public float speed;
        public float distance;
        public LayerMask enemyLayermask;

        [Header("SFXs")]
        public AudioSource attackerMoveSfx;

        private void Start()
        {
            attackerMoveSfx.Play();
        }
        public void Update()
        {
            this.transform.position += Vector3.forward * Time.deltaTime * speed;

            Attack();
        }

        public void Shoot()
        {
            //transform.DOMoveZ()
        }

        public void Attack()
        {
            if (Physics.Raycast(transform.position, Vector3.forward, out var hitInfo, distance, enemyLayermask))
            {
                BossManager.Instance.GetDamage();

                Destroy(gameObject);
            }
            
        }

        public void CheckToDestroy()
        {
            if (Vector3.Distance(transform.position, GridMapManager.Instance.gridMap.center) > destroyDistance)
            {
                Destroy(gameObject);
            }
        }

    }

}
