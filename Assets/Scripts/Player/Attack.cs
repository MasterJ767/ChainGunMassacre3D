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

        public Weapon GetCurrentWeapon()
        {
            return weapons[currentWeapon];
        }

        private void ChangeGun(int amount)
        {
            StartCoroutine(PreventSwitch());
            weapons[currentWeapon].gameObject.SetActive(false);
            currentWeapon = Modulo((currentWeapon + amount), weapons.Length);
            weapons[currentWeapon].gameObject.SetActive(true);
            Weapon weapon = weapons[currentWeapon].GetComponent<Weapon>();
            weapon.fireCooldown = 0.1f;
            weapon.SetAmmoText();
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
