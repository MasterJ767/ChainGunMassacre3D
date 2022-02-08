using UnityEngine;

namespace Item
{
    public class Controller : MonoBehaviour
    {
        public string itemName;

        public float multiplier;
        public float quantity;
        public float damage;
        public float range;
        public float force;
        public GameObject prefab;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ActivateEffect(other);
            }
        }

        private void ActivateEffect(Collider player)
        {
            
        }
    }
}
