using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class EffectEarth : MonoBehaviour
    {
        public Color baseColour;
        public Color tint;
        public MeshRenderer mesh;

        private Rigidbody rb;

        private Color tintColour;
        private EarthParameters parameters;
        private Weapon weapon;
        private int repeats;
        private GameObject emitter;

        private void Start()
        {
            tintColour = tint;
            mesh.material.SetColor(Shader.PropertyToID("_Colour"), baseColour);
            mesh.material.SetColor(Shader.PropertyToID("_Tint"), tintColour);
            
            rb = GetComponent<Rigidbody>();
        }

        public void Initialise(EarthParameters EarthParameters, Weapon Weapon, int Repeats, GameObject Emitter)
        {
            parameters = EarthParameters;
            weapon = Weapon;
            repeats = Repeats;
            emitter = Emitter;
        }

        private void Update()
        {
            tintColour.a = Mathf.Clamp01(tintColour.a - 0.4f * Time.deltaTime);
            mesh.material.SetColor(Shader.PropertyToID("_Tint"), tintColour);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Enemy") && other.gameObject != emitter)
            {
                Resource.Health enemyHealth = other.gameObject.GetComponent<Resource.Health>();
                enemyHealth.Damage(parameters.damage, parameters.bulletColour, DamageType.EARTH);

                if (repeats > 0 && other.gameObject != null && !enemyHealth.isDead)
                {
                    Enemy.Effects enemyEffects = other.gameObject.GetComponent<Enemy.Effects>();
                    enemyEffects.StartEarthAttack(parameters, weapon, repeats - 1);
                }
            }

            if (other.gameObject.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
