using UnityEngine;

namespace Item
{
    public class Controller : MonoBehaviour
    {
        public string itemName;
        
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
            switch (itemName)
            {
                case "Ammo Box":
                    AmmoBox(player);
                    break;
                case "Health Pack":
                    HealthPack(player);
                    break;
                case "Syringe":
                    Syringe(player);
                    break;
                case "Bullet Pulse":
                    BulletPulse(player);
                    break;
                case "Knockback Wave":
                    KnockbackWave(player);
                    break;
                default:
                    Debug.Log(name + " has no function yet");
                    break;
            }
        }
        
        private void AmmoBox(Collider player)
        {

        }
        
        private void HealthPack(Collider player)
        {
            Resource.Health playerHealth = player.gameObject.GetComponent<Resource.Health>();
            playerHealth.Heal(playerHealth.maxHealth);
        }

        private void Syringe(Collider player)
        {
            Resource.Stamina playerStamina = player.gameObject.GetComponent<Resource.Stamina>();
            playerStamina.Recover(playerStamina.maxStamina);
        }

        private void BulletPulse(Collider player)
        {
            
        }

        private void KnockbackWave(Collider player)
        {
            Collider[] enemies = Physics.OverlapSphere(player.transform.position, range);
            
            //Particles

            foreach (Collider enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.gameObject.GetComponent<Enemy.Movement>().Knockback(force);
                }
            }
        }
    }
}
