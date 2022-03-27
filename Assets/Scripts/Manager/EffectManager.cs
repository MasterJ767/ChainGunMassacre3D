using UnityEditor;
using UnityEngine;

namespace Manager
{
    public class EffectManager : MonoBehaviour
    {
        public bool showDamageNumbers = true;
        public bool showEffectDamageNumbers = true;
        
        private static EffectManager instance;

        public static EffectManager GetInstance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EffectManager>();
                if (instance == null)
                {
                    Debug.LogWarning("NO EFFECT MANAGER IN SCENE");
                }
            }

            return instance;
        }

        public void SetShowDamageNumbers(bool state)
        {
            showDamageNumbers = state;
        }

        public void SetShowEffectDamageNumbers(bool state)
        {
            showEffectDamageNumbers = state;
        }
    }
}
