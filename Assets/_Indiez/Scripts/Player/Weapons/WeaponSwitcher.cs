using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    public WeaponHolder weaponHolder;
    public List<BaseWeapon> availableWeapons = new List<BaseWeapon>();
    private int m_currentIndex = 0;

    private void Start()
    {
        if (availableWeapons.Count > 0)
            weaponHolder.EquipWeapon(availableWeapons[m_currentIndex]);
    }

    public void SwitchTo(int index)
    {
        if (index < 0 || index >= availableWeapons.Count) return;
        if (index == m_currentIndex) return;

        m_currentIndex = index;
        weaponHolder.EquipWeapon(availableWeapons[m_currentIndex]);
        Debug.Log($"Switched to {availableWeapons[m_currentIndex].WeaponStats.WeaponName}");
    }
}
