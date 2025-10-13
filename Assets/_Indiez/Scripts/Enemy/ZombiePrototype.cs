using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using FIMSpace.FProceduralAnimation;
using Premium;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class ZombiePrototype : EnemyBase, IDamageable
{
    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_HitDamgePelvisImpulse;
    [SerializeField, BoxGroup("References")] protected ZombieAIController m_ZombieAIController;
    [SerializeField, BoxGroup("References")] protected RagdollController m_RagdollController;
    [SerializeField, BoxGroup("References")] protected AnimateDissolve m_AnimateDissolve;
    [SerializeField, BoxGroup("Resource")] protected HealthBarSO m_HealthBarSO;
    [SerializeField, BoxGroup("Resource")] protected BulletImpactDataSO m_BulletImpactDataSO;
    [SerializeField, BoxGroup("Resource")] protected EnemyBase m_PrefabThis;

    protected List<Material> m_IndiMatSkins;
    protected SkinnedMeshRenderer[] m_SkinnedMeshRenderers;

    protected Dictionary<GameObject, Material> m_SaveMats;

    protected virtual void Awake()
    {
        m_SaveMats = new Dictionary<GameObject, Material>();
    }

    public override void Init(EnemyStatsSO statsSO = null)
    {
        base.Init(statsSO);
        InitInfo();
        InitDissolveMat();

        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();
        m_HealthBarMesh.material = new Material(m_HealthBarSO.OpponentHealthBarMaterial);
        RangeIntValue range = new RangeIntValue(0, m_EnemyStats.Health);
        var progress = new RangeProgress<int>(range, m_EnemyStats.Health);
        m_HealthBar.Init(progress);

        m_ZombieAIController.InitializeStateMachine();
    }

    protected virtual void InitInfo()
    {
        m_IsAlive = true;
        m_ZombieAIController.NavMeshAgent.enabled = true;
        m_Animator.enabled = true;
        m_LegsAnimator.enabled = m_IsAlive;
        m_HealthBarMesh.enabled = m_IsAlive;
        m_CharacterController.enabled = m_IsAlive;
        m_RagdollController.DisableRagdoll();
    }

    protected virtual void InitDissolveMat()
    {
        if (m_IndiMatSkins != null && m_IndiMatSkins.Count > 0)
            return;
        m_IndiMatSkins = new List<Material>();
        m_SkinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var skinMesh in m_SkinnedMeshRenderers)
        {
            Material[] mats = skinMesh.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null)
                    continue;

                Material instanceMat = null;
                if (m_SaveMats.TryGetValue(skinMesh.gameObject, out Material material))
                    instanceMat = new Material(material);
                else
                {
                    m_SaveMats.Add(skinMesh.gameObject, mats[i]);
                    instanceMat = new Material(mats[i]);
                }


                mats[i] = instanceMat;
                m_IndiMatSkins.Add(instanceMat);
            }

            skinMesh.materials = mats;
        }
    }



    protected void Dead()
    {
        m_IsAlive = false;
        OnDead?.Invoke(this);
        m_ZombieAIController.NavMeshAgent.enabled = false;
        m_Animator.ResetAllTriggers();
        ResetAnimator();
        m_Animator.enabled = false;
        m_LegsAnimator.enabled = m_IsAlive;
        m_HealthBarMesh.enabled = m_IsAlive;
        m_CharacterController.enabled = m_IsAlive;
        m_RagdollController.EnableRagdoll();
    }

    private void ResetAnimator()
    {
        var animatorController = m_Animator.runtimeAnimatorController;

        foreach (AnimatorControllerParameter param in m_Animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    m_Animator.SetBool(param.name, false);
                    break;
                case AnimatorControllerParameterType.Float:
                    m_Animator.SetFloat(param.name, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    m_Animator.SetInteger(param.name, 0);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    m_Animator.ResetTrigger(param.name);
                    break;
            }
        }

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
                float coolDownTime = 5;
                foreach (var dissolveMat in m_IndiMatSkins)
                    m_AnimateDissolve.PlayDissolve(dissolveMat, coolDownTime);
                m_SkinnedMeshRenderers.ForEach(v => v.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off);

                StartCoroutine(CommonCoroutine.Delay(coolDownTime, false, () =>
                {
                    m_SkinnedMeshRenderers.ForEach(v => v.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On);
                    ClearDissolveMats();
                    PoolManager.Release($"ZombiePrototype", gameObject.GetComponent<EnemyBase>());
                    gameObject.SetActive(false);
                }));

            }));
        }
        m_HealthBar.SetValue(m_EnemyStats.Health + (int)amount, m_EnemyStats.Health, 0.2f);
    }

    protected virtual void ClearDissolveMats()
    {
        if (m_IndiMatSkins == null) return;

        foreach (var mat in m_IndiMatSkins)
        {
            if (mat != null)
                Destroy(mat);
        }
        m_IndiMatSkins.Clear();
    }


#if UNITY_EDITOR
    [Button]
    public void TakeDameEditor(int amout)
    {
        TakeDamage(amout, Vector3.zero);
    }
#endif
}
