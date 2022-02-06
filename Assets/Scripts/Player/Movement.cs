using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        public float baseSpeed = 6f;
        public float multiplier = 1.05f;
        [HideInInspector] 
        public int speedLevel = 0;

        public Rigidbody rb;
        private Vector3 movement;

        private float speed => baseSpeed * Mathf.Pow(multiplier, speedLevel);

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + (movement * speed * Time.fixedDeltaTime));
        }

        private void Update()
        {
            GetMovementInputs();
        }

        private void GetMovementInputs()
        {
            movement = (Input.GetAxisRaw("Vertical") * transform.forward) + (Input.GetAxisRaw("Horizontal") * transform.right);
            movement = movement.normalized;
        }
    }
}
