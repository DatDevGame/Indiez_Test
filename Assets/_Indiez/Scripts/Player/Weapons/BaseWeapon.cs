using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public Transform RightHandIK => m_RightHandIK;
    public Transform LeftHandIK => m_LeftHandIK;
    public WeaponStats WeaponStats => weaponStats;
    public WeaponSO WeaponSO => weaponSO;

    [SerializeField, BoxGroup("Reference")] protected Transform m_RightHandIK;
    [SerializeField, BoxGroup("Reference")] protected Transform m_LeftHandIK;

    [SerializeField, BoxGroup("Weapon Data")] protected WeaponSO weaponSO;

    protected float m_nextFireTime;
    protected bool m_isEquipped;
    protected WeaponStats weaponStats;

    public virtual void OnEquip()
    {
        if (weaponSO != null)
            weaponStats = new WeaponStats(weaponSO);
    }

    public virtual void OnUnequip()
    {

    }

    public abstract void Fire();

    public virtual void Reload()
    {
        Debug.Log($"{weaponStats.WeaponName} reloaded!");
    }
}


public class WeaponStats
{
    public WeaponStats(WeaponSO weaponSO)
    {
        if (weaponSO.TryGetModule(out WeaponInfoModule module))
        {
            WeaponName = weaponSO.GetDisplayName();
            Damage = module.Damage;
            FireRate = module.FireRate;
            Range = module.Range;
            MaxAmmo = module.MaxAmmo;
            Ammo = module.MaxAmmo;
        }
    }
    public string WeaponName;
    public float Damage;
    public float FireRate;
    public float Range;
    public int Ammo;
    public int MaxAmmo;
}
