using System;
using System.Collections;
using System.Collections.Generic;
using FIMSpace.FProceduralAnimation;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public class ZombiePrototype : EnemyBase, IDamageable
{
    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_HitDamgePelvisImpulse;
    [SerializeField, BoxGroup("Resource")] protected HealthBarSO m_HealthBarSO;
    [SerializeField, BoxGroup("Resource")] protected BulletImpactDataSO m_BulletImpactDataSO;

    protected void Start()
    {
        Init(m_StatsSO);
    }

    public override void Init(EnemyStatsSO statsSO = null)
    {
        base.Init(statsSO);

        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();
        m_HealthBarMesh.material = new Material(m_HealthBarSO.OpponentHealthBarMaterial);
        RangeIntValue range = new RangeIntValue(0, m_EnemyStats.Health);
        var progress = new RangeProgress<int>(range, 100);
        m_HealthBar.Init(progress);
    }

    protected void Dead()
    {
        m_IsAlive = false;
        OnDead?.Invoke();
        m_Animator.SetTrigger(m_AnimationKeySO.DeadTrigger);
        m_Animator.SetBool(m_AnimationKeySO.Dead, true);
        m_LegsAnimator.enabled = m_IsAlive;
        m_HealthBarMesh.enabled = m_IsAlive;
        m_CharacterController.enabled = m_IsAlive;
    }

    public void TakeDamage(float amount, Vector3 hitPos)
    {
        if (m_EnemyStats.Health > 0)
        {
            m_EnemyStats.Health -= (int)amount;
            m_LegsAnimator.User_AddImpulse(m_HitDamgePelvisImpulse);

            #region Pool BulletImpact
            ParticleSystem bulletImpactPrefab = m_BulletImpactDataSO.GetBulletImpact(gameObject.layer);
            // --- Bullet Impact VFX ---
            var bulletImpactPool = PoolManager.GetOrCreatePool<ParticleSystem>(
                objectPrefab: bulletImpactPrefab,
                initialCapacity: 1
            );
            ParticleSystem bulletImpact = bulletImpactPool.Get();

            bulletImpact.transform.SetParent(transform, false);
            bulletImpact.transform.position = hitPos;

            bulletImpact.gameObject.SetActive(true);
            bulletImpact.Play();
            bulletImpact.Release(bulletImpactPrefab, 0.2f);
            #endregion
        }


        m_EnemyStats.Health -= (int)amount;
        if (m_EnemyStats.Health <= 0)
        {
            Dead();
            m_EnemyStats.Health = 0;
        }

        m_HealthBar.SetValue(m_EnemyStats.Health + (int)amount, m_EnemyStats.Health, 0.2f);
        Debug.Log($"Receive Attack");
    }

#if UNITY_EDITOR
    [Button]
    public void TakeDameEditor(int amout)
    {
        TakeDamage(amout, Vector3.zero);
    }
#endif
}
