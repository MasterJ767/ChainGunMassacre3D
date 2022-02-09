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
            Player.Attack playerWeaponController = player.gameObject.GetComponent<Player.Attack>();
            Player.Weapon[] weapons = playerWeaponController.weapons;
            foreach (Player.Weapon weapon in weapons)
            {
                weapon.Refill(weapon.maxAmmo);
            }
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
            Player.Weapon currentWeapon = player.gameObject.GetComponent<Player.Attack>().GetCurrentWeapon();
            
            Transform playerTransform = player.gameObject.transform;
            Vector3 up = playerTransform.up;
            Vector3 bottomTarget = playerTransform.position;
            Vector3 centre = bottomTarget + (up * 0.9f);
            Vector3 topTarget = bottomTarget + (up * 1.8f);
            float separationAngle = (2 * Mathf.PI / quantity);
            float shotSpeed = 10f;
            float spreadAngleRad = 30f * Mathf.Deg2Rad;

            for (int i = 0; i < quantity; i++)
            {
                Vector3 spreadDirection = (Mathf.Cos(i * separationAngle) * playerTransform.right) + (Mathf.Sin(i * separationAngle) * playerTransform.forward);
                float spreadDistance = 0.9f * Mathf.Tan(spreadAngleRad);
                Vector3 bottomImpactLocation = bottomTarget + (spreadDirection.normalized * spreadDistance);
                spreadDirection.y += 0.9f;

                InstantiateBullet(centre, Quaternion.Euler(spreadDirection), playerTransform, spreadDirection, shotSpeed, currentWeapon);
                
                spreadDirection.y += 0.9f;
                Vector3 topImpactLocation = topTarget + (spreadDirection.normalized * spreadDistance);
                Vector3 bottomBulletDirection = (bottomImpactLocation - centre).normalized;
                Vector3 topBulletDirection = (topImpactLocation - centre).normalized;
                
                InstantiateBullet(centre, Quaternion.Euler(bottomBulletDirection), playerTransform, bottomBulletDirection, shotSpeed, currentWeapon);
                InstantiateBullet(centre, Quaternion.Euler(topBulletDirection), playerTransform, topBulletDirection, shotSpeed, currentWeapon);
            }
            
        }

        private void InstantiateBullet(Vector3 posiiton, Quaternion rotation, Transform parent, Vector3 direction, float shotSpeed, Player.Weapon currentWeapon)
        {
            GameObject bulletGameObject = Instantiate(prefab, posiiton, rotation,parent);
            bulletGameObject.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
            bulletGameObject.GetComponent<Player.Projectile>().Initialise(currentWeapon, new Player.NoneParameters(new Color(1, 1, 0, 0f)), Player.ElementalEffect.NONE, damage, 6f, 2);
            Destroy(bulletGameObject, range / shotSpeed);
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
