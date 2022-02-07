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

        public float jumpHeight = 3f;
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        private bool isGrounded;
        
        public Rigidbody rb;
        private Vector3 movement;

        private float speed => baseSpeed * Mathf.Pow(multiplier, speedLevel);

        private void FixedUpdate()
        {
            //rb.MovePosition(rb.position + (movement * speed * Time.fixedDeltaTime));
            
            MovePlayer();
        }

        private void Update()
        {
            GroundedCheck();
            
            GetMovementInputs();

            JumpCheck();
        }

        private void GroundedCheck()
        {
            Vector3 groundCheckPosition = groundCheck.position;
            isGrounded = Physics.CheckCapsule(groundCheckPosition, groundCheckPosition - new Vector3(0, groundDistance, 0), 0.4f);
        }

        private void GetMovementInputs()
        {
            movement = (Input.GetAxisRaw("Vertical") * transform.forward) + (Input.GetAxisRaw("Horizontal") * transform.right);
            movement = movement.normalized;
        }

        private void JumpCheck()
        {
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                float f = Mathf.Sqrt(-jumpHeight * Physics.gravity.y);
                rb.AddForce(Vector3.up * f, ForceMode.Impulse);
            }
        }

        private void MovePlayer()
        {
            if (Input.GetButton("Sprint"))
            {
                Resource.Stamina stamina = gameObject.GetComponent<Resource.Stamina>();
                if (stamina.Expend(0.5f))
                {
                    rb.MovePosition(rb.position + (movement * speed * 2f * Time.fixedDeltaTime));
                    return;
                }
            }
            
            rb.MovePosition(rb.position + (movement * speed * Time.fixedDeltaTime));
        }
    }
}
