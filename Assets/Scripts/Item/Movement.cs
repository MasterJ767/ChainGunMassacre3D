using System;
using UnityEngine;

namespace Item
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        private Transform target;
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            target = transform;
        }

        private void Update()
        {
            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = transform;
            }
        }
        
        private void Move()
        {
            Vector3 step = (target.position - transform.position).normalized;
            rb.velocity = step * 5f;
        }
    }
}
