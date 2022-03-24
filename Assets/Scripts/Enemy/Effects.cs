using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Enemy
{
    public class Effects : MonoBehaviour
    {
        public Color colour;
        public Color fireTint;
        public Color iceTint;
        public Color poisonTint;
        public Color earthTint;
        public Material material;
        
        private Dictionary<ElementalEffect, EffectInformation> effectStatus;
        private Color tintColor;
        private bool fadeActive = false;
        private float tintFadeSpeed = 6f;

        private void Awake()
        {
            effectStatus = new Dictionary<ElementalEffect, EffectInformation>();
            effectStatus.Add(ElementalEffect.FIRE, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));
            effectStatus.Add(ElementalEffect.ICE, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));
            effectStatus.Add(ElementalEffect.POISON, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));
            effectStatus.Add(ElementalEffect.EARTH, new EffectInformation(false, new NoneParameters(new Color(1, 1, 0, 0f)), null));

            tintColor = new Color(1, 0, 0, 0);
        }

        private void Update()
        {
            if (fadeActive && tintColor.a > 0)
            {
                tintColor.a = Mathf.Clamp01(tintColor.a - tintFadeSpeed * Time.deltaTime);
                material.SetColor(Shader.PropertyToID("_Tint"), tintColor);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (effectStatus[ElementalEffect.FIRE].active && other.gameObject.CompareTag("Enemy"))
            {
                Effects enemyEffects = other.gameObject.GetComponent<Effects>();
                enemyEffects.StartFireAttack((FireParameters)effectStatus[ElementalEffect.FIRE].parameters, effectStatus[ElementalEffect.FIRE].weapon);
            }
        }

        public void StartFireAttack(FireParameters parameters, Weapon weapon)
        {
            tintColor = fireTint;
            material.SetColor(Shader.PropertyToID("_Tint"), tintColor);

            effectStatus[ElementalEffect.FIRE].active = true;
            effectStatus[ElementalEffect.FIRE].parameters = parameters;
            effectStatus[ElementalEffect.FIRE].weapon = weapon;
        }

        public void StartIceAttack(IceParameters parameters, Weapon weapon)
        {
            
        }

        public void StartPoisonAttack(PoisonParameters parameters, Weapon weapon)
        {
            
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
