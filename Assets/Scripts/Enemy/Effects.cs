using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class Effects : MonoBehaviour
    {
        public Color baseColour;
        public Gradient baseGradient;
        public Material baseMaterial;
        public Color fireTint;
        public Gradient fireTrailGradient;
        public Material fireMaterial;
        public Color iceTint;
        public Gradient iceTrailGradient;
        public Material iceMaterial;
        public Color poisonTint;
        public Gradient poisonTrailGradient;
        public Material poisonMaterial;
        public Color earthTint;
        public GameObject rock;
        public SkinnedMeshRenderer mesh;
        public ParticleSystem trail;

        private Dictionary<ElementalEffect, EffectInformation> effectStatus;
        private Color tintColour;
        private bool fadeTint = false;
        private float tintFadeSpeed = 5f;
        
        private Manager.EffectManager em;

        private void Awake()
        {
            effectStatus = new Dictionary<ElementalEffect, EffectInformation>();
            effectStatus.Add(ElementalEffect.FIRE, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null, 0));
            effectStatus.Add(ElementalEffect.ICE, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null,0));
            effectStatus.Add(ElementalEffect.POISON, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null,0));
            effectStatus.Add(ElementalEffect.EARTH, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null,0));

            em = Manager.EffectManager.GetInstance();
            
            SetColour(baseColour);
            tintColour = new Color(baseColour.r, baseColour.g, baseColour.b, 0);
        }

        private void Update()
        {
            if (fadeTint && tintColour.a > 0)
            {
                tintColour.a = Mathf.Clamp01(tintColour.a - tintFadeSpeed * Time.deltaTime);
                SetTint(tintColour);
            }
            else if (tintColour.a <= 0)
            {
                fadeTint = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (effectStatus[ElementalEffect.FIRE].active && other.gameObject.CompareTag("Enemy"))
            {
                Effects enemyEffects = other.gameObject.GetComponent<Effects>();
                enemyEffects.StartFireAttack((FireParameters)effectStatus[ElementalEffect.FIRE].parameters, effectStatus[ElementalEffect.FIRE].weapon);
            }
            else if (effectStatus[ElementalEffect.ICE].active && other.gameObject.CompareTag("Enemy"))
            {
                Effects enemyEffects = other.gameObject.GetComponent<Effects>();
                enemyEffects.StartIceAttack((IceParameters)effectStatus[ElementalEffect.ICE].parameters, effectStatus[ElementalEffect.ICE].weapon);
            }
        }

        private void SetColour(Color colour)
        {
            mesh.material.SetColor(Shader.PropertyToID("_Colour"), colour);
        }

        private void SetTint(Color colour)
        {
            tintColour = colour;
            mesh.material.SetColor(Shader.PropertyToID("_Tint"), tintColour);
        }

        private void SetTrail(Gradient gradient, Material material)
        {
            Renderer trailRenderer = trail.GetComponent<Renderer>();
            trailRenderer.material = material;
            ParticleSystem.ColorOverLifetimeModule trailColour = trail.colorOverLifetime;
            trailColour.color = gradient;
        }

        private void ToggleTrail(bool state)
        {
            if (state)
            {
                trail.Play();
            }
            else
            {
                trail.Stop();
                SetTrail(baseGradient, baseMaterial);
            }
        }

        public void StartFireAttack(FireParameters parameters, Weapon weapon)
        {
            if (effectStatus[ElementalEffect.EARTH].active || effectStatus[ElementalEffect.FIRE].active)
            {
                return;
            }

            fadeTint = false;

            if (effectStatus[ElementalEffect.ICE].active)
            {
                ClearIce();
            }
            
            if (effectStatus[ElementalEffect.POISON].active)
            {
                ClearPoison();
            }
            
            SetTint(fireTint);
            SetTrail(fireTrailGradient, fireMaterial);
            ToggleTrail(true);

            effectStatus[ElementalEffect.FIRE].active = true;
            effectStatus[ElementalEffect.FIRE].parameters = parameters;
            effectStatus[ElementalEffect.FIRE].weapon = weapon;

            StartCoroutine(nameof(FireAttack));
        }

        private void ClearFire()
        {
            StopCoroutine(nameof(FireAttack));
            GetComponent<Movement>().speedLevel -= ((FireParameters)effectStatus[ElementalEffect.FIRE].parameters).speedBoost;
            ToggleTrail(false);
            SetTint(new Color(tintColour.r, tintColour.g, tintColour.b, 0));
            effectStatus[ElementalEffect.FIRE].active = false;
        }

        private IEnumerator FireAttack()
        {
            Movement movement = GetComponent<Movement>();
            Resource.Health health = GetComponent<Resource.Health>();
            FireParameters parameters = (FireParameters)effectStatus[ElementalEffect.FIRE].parameters; 

            movement.speedLevel += parameters.speedBoost;

            float time = parameters.duration;

            while (time > 0)
            {
                health.Damage(parameters.damage, parameters.bulletColour, DamageType.FIRE);
                
                time -= parameters.delay;
                yield return new WaitForSeconds(parameters.delay);
            }
            
            movement.speedLevel -= parameters.speedBoost;
            ToggleTrail(false);
            fadeTint = true;
            effectStatus[ElementalEffect.FIRE].active = false;
        }

        public void StartIceAttack(IceParameters parameters, Weapon weapon)
        {
            if (effectStatus[ElementalEffect.EARTH].active || effectStatus[ElementalEffect.ICE].active)
            {
                return;
            }
            
            fadeTint = false;
            
            if (effectStatus[ElementalEffect.FIRE].active)
            {
                ClearFire();
            }

            if (effectStatus[ElementalEffect.POISON].active)
            {
                ClearPoison();
            }

            SetTint(iceTint);
            SetTrail(iceTrailGradient, iceMaterial);
            ToggleTrail(true);
            
            effectStatus[ElementalEffect.ICE].active = true;
            effectStatus[ElementalEffect.ICE].parameters = parameters;
            effectStatus[ElementalEffect.ICE].weapon = weapon;

            StartCoroutine(nameof(IceAttack));
        }

        private void ClearIce()
        {
            StopCoroutine(nameof(IceAttack));
            GetComponent<Movement>().speedLevel += ((IceParameters)effectStatus[ElementalEffect.ICE].parameters).speedDrop;
            ToggleTrail(false);
            SetTint(new Color(tintColour.r, tintColour.g, tintColour.b, 0));
            effectStatus[ElementalEffect.ICE].active = false;
        }

        private IEnumerator IceAttack()
        {
            Movement movement = GetComponent<Movement>();
            IceParameters parameters = (IceParameters)effectStatus[ElementalEffect.ICE].parameters; 

            movement.speedLevel -= parameters.speedDrop;

            yield return new WaitForSeconds(parameters.duration);
            
            movement.speedLevel += parameters.speedDrop;
            ToggleTrail(false);
            fadeTint = true;
            effectStatus[ElementalEffect.ICE].active = false;
        }

        public void StartPoisonAttack(PoisonParameters parameters, Weapon weapon)
        {
            if (effectStatus[ElementalEffect.EARTH].active || effectStatus[ElementalEffect.POISON].active)
            {
                return;
            }
            
            fadeTint = false;
            
            if (effectStatus[ElementalEffect.FIRE].active)
            {
                ClearFire();
            }

            if (effectStatus[ElementalEffect.ICE].active)
            {
                ClearIce();
            }

            SetTint(poisonTint);
            SetTrail(poisonTrailGradient, poisonMaterial);
            ToggleTrail(true);
            
            effectStatus[ElementalEffect.POISON].active = true;
            effectStatus[ElementalEffect.POISON].parameters = parameters;
            effectStatus[ElementalEffect.POISON].weapon = weapon;

            StartCoroutine(nameof(PoisonAttack));
        }

        private void ClearPoison()
        {
            StopCoroutine(nameof(PoisonAttack));
            ToggleTrail(false);
            SetTint(new Color(tintColour.r, tintColour.g, tintColour.b, 0));
            effectStatus[ElementalEffect.POISON].active = false;
        }

        private IEnumerator PoisonAttack()
        {
            Resource.Health health = GetComponent<Resource.Health>();
            PoisonParameters parameters = (PoisonParameters)effectStatus[ElementalEffect.POISON].parameters; 
            
            float time = parameters.duration;

            while (time > 0)
            {
                health.Damage(parameters.damage, parameters.bulletColour, DamageType.POISON);
                
                time -= parameters.delay;
                yield return new WaitForSeconds(parameters.delay);
            }

            ToggleTrail(false);
            fadeTint = true;
            effectStatus[ElementalEffect.POISON].active = false;
        }

        public void StartEarthAttack(EarthParameters parameters, Weapon weapon, int repeats)
        {
            if (effectStatus[ElementalEffect.EARTH].active)
            {
                return;
            }

            fadeTint = false;
            
            if (effectStatus[ElementalEffect.FIRE].active)
            {
                ClearFire();
            }

            if (effectStatus[ElementalEffect.ICE].active)
            {
                ClearIce();
            }
            
            if (effectStatus[ElementalEffect.POISON].active)
            {
                ClearPoison();
            }

            SetColour(earthTint);
            SetTint(earthTint);
            
            effectStatus[ElementalEffect.EARTH].active = true;
            effectStatus[ElementalEffect.EARTH].parameters = parameters;
            effectStatus[ElementalEffect.EARTH].weapon = weapon;
            effectStatus[ElementalEffect.EARTH].repeats = repeats;
            
            StartCoroutine(nameof(EarthAttack));
        }
        
        private void ClearEarth()
        {
            StopCoroutine(nameof(EarthAttack));
            GetComponentInChildren<Attack>().EnableAttack();
            GetComponent<Movement>().EnableMovement();
            SetColour(baseColour);
            SetTint(new Color(tintColour.r, tintColour.g, tintColour.b, 0));
            effectStatus[ElementalEffect.EARTH].active = false;
        }

        private IEnumerator EarthAttack()
        {
            Movement movement = GetComponent<Movement>();
            Attack attack = GetComponentInChildren<Attack>();
            EarthParameters parameters = (EarthParameters)effectStatus[ElementalEffect.EARTH].parameters;
            
            movement.DisableMovement();
            attack.DisableAttack();

            float time = parameters.duration;

            while (time > 0)
            {
                Vector3 origin = transform.position;
                float angle = (2 * Mathf.PI * Random.value);
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized * (parameters.range / 2f);
                float elevation = direction.magnitude * Mathf.Tan(80);
                Vector3 apex = origin + direction + new Vector3(0, elevation, 0);
                Vector3 trajectory = (apex - origin);
                GameObject rockGO = Instantiate(rock, origin + new Vector3(0, 1f, 0), Quaternion.identity, em.transform);
                rockGO.GetComponent<EffectEarth>().Initialise(parameters, effectStatus[ElementalEffect.EARTH].weapon, effectStatus[ElementalEffect.EARTH].repeats, gameObject);
                rockGO.GetComponent<Rigidbody>().velocity = trajectory;
                
                time -= parameters.delay;
                yield return new WaitForSeconds(parameters.delay);
            }
            
            attack.EnableAttack();
            movement.EnableMovement();
            SetColour(baseColour);
            fadeTint = true;
            effectStatus[ElementalEffect.EARTH].active = false;
        }

        public ElementalEffect GetActiveEffect()
        {
            if (effectStatus[ElementalEffect.FIRE].active)
            {
                return ElementalEffect.FIRE;
            }

            if (effectStatus[ElementalEffect.ICE].active)
            {
                return ElementalEffect.ICE;
            }

            if (effectStatus[ElementalEffect.POISON].active)
            {
                return ElementalEffect.POISON;
            }

            if (effectStatus[ElementalEffect.EARTH].active)
            {
                return ElementalEffect.EARTH;
            }

            return ElementalEffect.NONE;
        }

        public bool GetActiveEffectStatus(ElementalEffect effectType)
        {
            if (!effectStatus.ContainsKey(effectType))
            {
                return false;
            }

            return effectStatus[effectType].active;
        }

        public EffectInformation GetEffectInformation(ElementalEffect effectType)
        {
            if (!effectStatus.ContainsKey(effectType))
            {
                return null;
            }

            return effectStatus[effectType];
        }
    }

    public class EffectInformation
    {
        public bool active;
        public ElementalParameters parameters;
        public Weapon weapon;
        public int repeats;

        public EffectInformation(bool activeStatus, ElementalParameters elementalParameters, Weapon weaponInformation, int repeatCount)
        {
            active = activeStatus;
            parameters = elementalParameters;
            weapon = weaponInformation;
            repeats = repeatCount;
        }
    }
}
