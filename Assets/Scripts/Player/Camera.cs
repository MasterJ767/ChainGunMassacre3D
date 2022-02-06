using UnityEngine;

namespace Player
{
    public class Camera : MonoBehaviour
    {
        public float mouseSensitivity = 100f;
        public Transform playerBody;
        public Transform playerHead;
        
        private float XRotation = 0f;
        
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

            XRotation -= mouseY;
            XRotation = Mathf.Clamp(XRotation, -90f, 90f);
            
            transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
            playerHead.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
            
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
