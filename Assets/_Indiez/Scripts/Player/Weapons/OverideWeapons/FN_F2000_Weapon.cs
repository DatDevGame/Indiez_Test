using System.Collections;
using System.Collections.Generic;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public class FN_F2000_Weapon : BaseWeapon
{
    [Button]
    public override void Fire()
    {
        var bulletPool = PoolManager.GetOrCreatePool<BaseBullet>(
            objectPrefab: BulletPrefab,
            initialCapacity: 1
        );

        BaseBullet bullet = bulletPool.Get();
        bullet.transform.SetPositionAndRotation(m_PointFire.position, m_PointFire.rotation);

        var firePool = PoolManager.GetOrCreatePool<ParticleSystem>(
            objectPrefab: m_BulletMuzzleFirePrefab,
            initialCapacity: 1
        );
        ParticleSystem fireVFX = firePool.Get();
        fireVFX.transform.SetPositionAndRotation(m_PointFire.position, m_PointFire.rotation);
        fireVFX.gameObject.SetActive(true);
        fireVFX.Play();
        fireVFX.Release(m_BulletMuzzleFirePrefab, 0.2f);

        bullet.OnInit(this);
        bullet.Shoot();
    }
}
