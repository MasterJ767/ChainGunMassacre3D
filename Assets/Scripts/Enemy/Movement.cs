using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        public float baseSpeed = 3f;
        public float multiplier = 1.05f;
        [HideInInspector] 
        public int speedLevel = 0;
        
        public Rigidbody rb;

        public float viewDistance = 20f;
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
            step.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(step.x, step.y, step.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            rb.velocity += (step * (speed * Mathf.Clamp01(speed - rb.velocity.magnitude)));
        }
        
        public void DisableMovement()
        {
            canMove = false;
            rb.mass = 1000;
        }

        public void EnableMovement()
        {
            canMove = true;
            rb.mass = 1;
        }

        public void Knockback(float force)
        {
            if (canKnockback)
            {
                rb.velocity = -rb.velocity * force;
                StartCoroutine(DisableKnockback());
            }
        }

        public void Knockback(Vector3 force)
        {
            if (canKnockback)
            {
                rb.velocity = force;
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
