using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Enemy
{
    public class Effects : MonoBehaviour
    {
        public Color baseColour;
        public Color fireTint;
        public Gradient fireTrailGradient;
        public Color iceTint;
        public Gradient iceTrailGradient;
        public Color poisonTint;
        public Gradient poisonTrailGradient;
        public Color earthTint;
        public SkinnedMeshRenderer mesh;
        public ParticleSystem trail;
        
        private Dictionary<ElementalEffect, EffectInformation> effectStatus;
        private Color tintColor;
        private bool fadeTint = false;
        private float tintFadeSpeed = 6f;

        private void Awake()
        {
            effectStatus = new Dictionary<ElementalEffect, EffectInformation>();
            effectStatus.Add(ElementalEffect.FIRE, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));
            effectStatus.Add(ElementalEffect.ICE, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));
            effectStatus.Add(ElementalEffect.POISON, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));
            effectStatus.Add(ElementalEffect.EARTH, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));

            tintColor = new Color(baseColour.r, baseColour.g, baseColour.b, 0);
        }

        private void Update()
        {
            if (fadeTint && tintColor.a > 0)
            {
                tintColor.a = Mathf.Clamp01(tintColor.a - tintFadeSpeed * Time.deltaTime);
                SetTint(tintColor);
            }
            else if (tintColor.a <= 0)
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

        private void SetTint(Color colour)
        {
            tintColor = colour;
            mesh.material.SetColor(Shader.PropertyToID("_Tint"), tintColor);
        }

        private void SetTrail(Gradient gradient)
        {
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
            SetTrail(fireTrailGradient);
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
            SetTint(new Color(tintColor.r, tintColor.g, tintColor.b, 0));
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
                health.Damage(parameters.damage, parameters.bulletColour);
                
                time -= parameters.frequency;
                yield return new WaitForSeconds(parameters.frequency);
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
            SetTrail(iceTrailGradient);
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
            SetTint(new Color(tintColor.r, tintColor.g, tintColor.b, 0));
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
            SetTrail(poisonTrailGradient);
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
            SetTint(new Color(tintColor.r, tintColor.g, tintColor.b, 0));
            effectStatus[ElementalEffect.POISON].active = false;
        }

        private IEnumerator PoisonAttack()
        {
            Resource.Health health = GetComponent<Resource.Health>();
            PoisonParameters parameters = (PoisonParameters)effectStatus[ElementalEffect.POISON].parameters; 
            
            float time = parameters.duration;

            while (time > 0)
            {
                health.Damage(parameters.damage, parameters.bulletColour);
                
                time -= parameters.frequency;
                yield return new WaitForSeconds(parameters.frequency);
            }

            ToggleTrail(false);
            fadeTint = true;
            effectStatus[ElementalEffect.POISON].active = false;
        }

        public void StartEarthAttack(EarthParameters parameters, Weapon weapon)
        {
            
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
    }

    public class EffectInformation
    {
        public bool active;
        public ElementalParameters parameters;
        public Weapon weapon;

        public EffectInformation(bool activeStatus, ElementalParameters elementalParameters, Weapon weaponInformation)
        {
            active = activeStatus;
            parameters = elementalParameters;
            weapon = weaponInformation;
        }
    }
}
