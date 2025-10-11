using System.Collections;
using System.Collections.Generic;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public class FN_F2000_Weapon : BaseWeapon
{
    public override void Fire()
    {
        // --- Bullet ---
        var bulletPool = PoolManager.GetOrCreatePool<BaseBullet>(
            objectPrefab: BulletPrefab,
            initialCapacity: 1
        );

        BaseBullet bullet = bulletPool.Get();

        // --- Fire VFX ---
        var firePool = PoolManager.GetOrCreatePool<ParticleSystem>(
            objectPrefab: m_BulletMuzzleFirePrefab,
            initialCapacity: 1
        );

        ParticleSystem fireVFX = firePool.Get();

        fireVFX.transform.SetParent(m_PointFire, false);
        fireVFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        fireVFX.transform.localScale = Vector3.one;

        fireVFX.gameObject.SetActive(true);
        fireVFX.Play();
        fireVFX.Release(m_BulletMuzzleFirePrefab, 0.2f);

        bullet.transform.SetPositionAndRotation(
            m_FakePoinfire.position,
            m_FakePoinfire.rotation
        );
        bullet.gameObject.SetActive(true);
        bullet.gameObject.layer = m_Owner.gameObject.layer;
        bullet.OnInit(this);
        bullet.Shoot();
    }
}
