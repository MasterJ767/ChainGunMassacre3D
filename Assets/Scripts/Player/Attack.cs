using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class Attack : MonoBehaviour
    {
        public Weapon[] weapons;
        
        private int currentWeapon = 0;
        private bool canSwitchWeapon = true;

        private void Start()
        {
            weapons[currentWeapon].gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Input.mouseScrollDelta.y > 0 && canSwitchWeapon)
            {
                ChangeGun(-1);
            }
            else if (Input.mouseScrollDelta.y < 0 && canSwitchWeapon)
            {
                ChangeGun(1);
            }
        }

        private void ChangeGun(int amount)
        {
            StartCoroutine(PreventSwitch());
            weapons[currentWeapon].gameObject.SetActive(false);
            currentWeapon = Modulo((currentWeapon + amount), weapons.Length);
            weapons[currentWeapon].gameObject.SetActive(true);
            weapons[currentWeapon].GetComponent<Weapon>().fireCooldown = 0.1f;
        }

        private int Modulo(int n, int m)
        {
            int r = n % m;
            return r < 0 ? r + m : r;
        }

        private IEnumerator PreventSwitch()
        {
            canSwitchWeapon = false;
            yield return new WaitForSeconds(0.1f);
            canSwitchWeapon = true;
        }
    }
}
