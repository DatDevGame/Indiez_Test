using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class AK47_Weapon : BaseWeapon
{
    private Soldier_1 m_Soldier_1;
    public override void Fire()
    {
        INavigationPoint navigationPoint = (INavigationPoint)m_Owner;
        if (m_Soldier_1 == null)
            m_Soldier_1 = (Soldier_1)m_Owner;

        Vector3 targetPoint = navigationPoint.GetTargetPoint();
        Vector3 dirToTarget = (targetPoint - m_FakePoinfire.position).normalized;

        dirToTarget = ApplyRecoilSpread(dirToTarget, weaponSO.RandomAngleRecol);

        Quaternion targetRot = Quaternion.LookRotation(dirToTarget);

        // --- Bullet Pool ---
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

        // --- Bullet setup ---
        bullet.transform.SetPositionAndRotation(
            m_FakePoinfire.position,
            targetRot
        );
        bullet.gameObject.SetActive(true);
        bullet.gameObject.layer = m_Owner.gameObject.layer;
        bullet.OnInit(this);
        bullet.Shoot();
        SoundManager.Instance.PlaySFX(ZWSoundSFX.AKM, 0.1f);
    }

    /// <summary>
    /// Tạo độ lệch ngẫu nhiên nhỏ quanh hướng bắn chính.
    /// </summary>
    private Vector3 ApplyRecoilSpread(Vector3 direction, float maxAngle)
    {
        // Random trong hình nón có góc maxAngle
        float angle = Random.Range(-maxAngle, maxAngle);
        float yaw = Random.Range(-maxAngle, maxAngle);

        Quaternion spreadRot = Quaternion.Euler(angle, yaw, 0f);
        return spreadRot * direction;
    }
}
