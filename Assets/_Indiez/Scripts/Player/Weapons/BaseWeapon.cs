using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public Transform RightHandIK => m_RightHandIK;
    public Transform LeftHandIK => m_LeftHandIK;
    public Transform PointFire => m_PointFire;
    public WeaponStats WeaponStats => weaponStats;
    public BaseBullet BulletPrefab => m_BulletPrefab;
    public WeaponSO WeaponSO => weaponSO;

    [SerializeField, BoxGroup("Reference")] protected Transform m_PointFire;
    [SerializeField, BoxGroup("Reference")] protected Transform m_RightHandIK;
    [SerializeField, BoxGroup("Reference")] protected Transform m_LeftHandIK;
    [SerializeField, BoxGroup("Resource")] protected BaseBullet m_BulletPrefab;
    [SerializeField, BoxGroup("Resource")] protected ParticleSystem m_BulletMuzzleFirePrefab;
    [SerializeField, BoxGroup("Weapon Data")] protected WeaponSO weaponSO;

    protected float m_nextFireTime;
    protected bool m_isEquipped;
    protected BaseSoldier m_Owner;
    protected Transform m_FakePoinfire;
    protected WeaponStats weaponStats;

    public virtual void OnEquip()
    {
        if (weaponSO != null)
            weaponStats = new WeaponStats(weaponSO);
    }

    public virtual void OnUnequip()
    {

    }

    [Button]
    public abstract void Fire();

    public virtual void Reload()
    {
        Debug.Log($"{weaponStats.WeaponName} reloaded!");
    }
    public virtual void SetFakePointFire(Transform FakePoinfire) => m_FakePoinfire = FakePoinfire;
    public virtual void SetOwner(BaseSoldier Owner) => m_Owner = Owner;
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
