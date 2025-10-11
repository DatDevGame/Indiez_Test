using System;
using System.Collections;
using System.Collections.Generic;
using FIMSpace.FProceduralAnimation;
using Premium;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ZombiePrototype : EnemyBase, IDamageable
{
    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_HitDamgePelvisImpulse;
    [SerializeField, BoxGroup("References")] protected ZombieAIController m_ZombieAIController;
    [SerializeField, BoxGroup("References")] protected RagdollController m_RagdollController;
    [SerializeField, BoxGroup("References")] protected AnimateDissolve m_AnimateDissolve;
    [SerializeField, BoxGroup("Resource")] protected HealthBarSO m_HealthBarSO;
    [SerializeField, BoxGroup("Resource")] protected BulletImpactDataSO m_BulletImpactDataSO;

    protected List<Material> m_IndiMatSkins;
    protected SkinnedMeshRenderer[] m_SkinnedMeshRenderers;

    protected void Start()
    {
        Init(m_StatsSO);
    }

    public override void Init(EnemyStatsSO statsSO = null)
    {
        base.Init(statsSO);
        InitDissolveMat();

        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();
        m_HealthBarMesh.material = new Material(m_HealthBarSO.OpponentHealthBarMaterial);
        RangeIntValue range = new RangeIntValue(0, m_EnemyStats.Health);
        var progress = new RangeProgress<int>(range, 100);
        m_HealthBar.Init(progress);
    }

    protected virtual void InitDissolveMat()
    {
        m_IndiMatSkins = new List<Material>();

        m_SkinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var skinMesh in m_SkinnedMeshRenderers)
        {
            Material[] mats = skinMesh.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null)
                    continue;

                Material instanceMat = new Material(mats[i]);
                mats[i] = instanceMat;
                m_IndiMatSkins.Add(instanceMat);
            }
            skinMesh.materials = mats;
        }
    }


    protected void Dead()
    {
        m_IsAlive = false;
        OnDead?.Invoke();
        m_ZombieAIController.NavMeshAgent.enabled = false;
        m_Animator.enabled = false;
        m_LegsAnimator.enabled = m_IsAlive;
        m_HealthBarMesh.enabled = m_IsAlive;
        m_CharacterController.enabled = m_IsAlive;

        m_RagdollController.EnableRagdoll();
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

        if (m_EnemyStats.Health <= 0)
        {
            Dead();
            m_EnemyStats.Health = 0;

            StartCoroutine(CommonCoroutine.Delay(3, false, () =>
            {
                foreach (var dissolveMat in m_IndiMatSkins)
                    m_AnimateDissolve.PlayDissolve(dissolveMat, 5f);
                m_SkinnedMeshRenderers.ForEach(v => v.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off);
            }));
        }
        m_HealthBar.SetValue(m_EnemyStats.Health + (int)amount, m_EnemyStats.Health, 0.2f);
    }

#if UNITY_EDITOR
    [Button]
    public void TakeDameEditor(int amout)
    {
        TakeDamage(amout, Vector3.zero);
    }
#endif
}
