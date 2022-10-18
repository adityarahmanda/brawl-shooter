using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBomber
{
    public class WeaponSelector : MonoBehaviour
    {
        [SerializeField] private Weapon[] m_weapons;

        private Animator m_animator;

        private int m_selectedWeaponIndex;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // m_selectedWeaponIndex = GetWeaponIndex(GameManager.instance.weaponType);

            for (int i = 0; i < m_weapons.Length; i++)
            {
                if (i == m_selectedWeaponIndex)
                {
                    m_weapons[i].gameObject.SetActive(true);
                }
                else
                {
                    m_weapons[i].gameObject.SetActive(false);
                }
            }

            ResetAnimation();
        }

        public void ToggleWeapon()
        {
            m_weapons[m_selectedWeaponIndex].gameObject.SetActive(false);
            m_selectedWeaponIndex++;

            if (m_selectedWeaponIndex >= m_weapons.Length)
            {
                m_selectedWeaponIndex = 0;
            }

            m_weapons[m_selectedWeaponIndex].gameObject.SetActive(true);
            // GameManager.instance.weaponType = m_weapons[m_selectedWeaponIndex].type;
            ResetAnimation();
        }

        private void ResetAnimation()
        {
            switch (m_weapons[m_selectedWeaponIndex].type)
            {
                case WeaponType.SingleShot:
                    m_animator.SetTrigger("gun");
                    break;
                case WeaponType.RapidShot:
                    m_animator.SetTrigger("machineGun");
                    break;
            }
        }

        private int GetWeaponIndex(WeaponType type)
        {
            for (int i = 0; i < m_weapons.Length; i++)
            {
                if (m_weapons[i].type == type)
                    return i;
            }

            return -1;
        }
    }
}
