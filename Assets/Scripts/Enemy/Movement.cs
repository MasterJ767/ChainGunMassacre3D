using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        public float baseSpeed = 6f;
        public float multiplier = 1.05f;
        [HideInInspector] 
        public int speedLevel = 0;
        
        public Rigidbody rb;

        public float viewDistance;
        public Transform target;
        [HideInInspector] 
        public float distanceToTarget;

        private bool canKnockback = true;
        private bool canMove = true;
        
        private float speed => baseSpeed * Mathf.Pow(multiplier, speedLevel);

        private void Update()
        {
            CalculateTargetDistance();
            
            Move();
        }

        private void CalculateTargetDistance()
        {
            distanceToTarget = (transform.position - target.position).magnitude;
        }

        private void Move()
        {
            if (distanceToTarget > viewDistance)
            {
                return;
            }

            if (!canMove)
            {
                return;
            }

            Vector3 step = (target.position - transform.position).normalized;
            rb.velocity += (step * (speed * Mathf.Clamp01(speed - rb.velocity.magnitude)));
        }

        public void Knockback(float force)
        {
            if (canKnockback)
            {
                rb.AddForce(-rb.velocity * force, ForceMode.Impulse);
                StartCoroutine(DisableKnockback());
            }
        }
        
        private IEnumerator DisableKnockback()
        {
            canKnockback = false;
            yield return new WaitForSeconds(0.35f);
            canKnockback = true;
        }
    }
}
