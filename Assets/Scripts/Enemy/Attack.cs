using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Attack : MonoBehaviour
    {
        public float attackDelay;
        public float damage;
        
        private bool canAttack = true;

        private void OnTriggerStay(Collider other)
        {
            if (canAttack && other.CompareTag("Player"))
            {
                StartCoroutine(AttackCooldown());
                
                other.gameObject.GetComponent<Resource.Health>().Damage(damage, Player.DamageType.ENEMY);
            }
        }

        private IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(attackDelay);
            canAttack = true;
        }
    }
}
