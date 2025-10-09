using System;
using System.Collections;
using System.Collections.Generic;
using FIMSpace.FProceduralAnimation;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Action OnDead = delegate { };
    public CharacterController CharacterController => m_CharacterController;
    public ZombieAnimationEventReceiver ZombieAnimationEventReceiver => m_ZombieAnimationEventReceiver;
    public Animator Animator => m_Animator;
    public EnemyStats EnemyStats => m_EnemyStats;
    public EnemyStatsSO StatsSOData => m_StatsSO;
    public ZombieAnimationSO ZombieAnimationSO => m_AnimationKeySO;
    public bool IsAlive => m_IsAlive;
    public bool IsLocal => m_IsLocal;

    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_BlockHit;
    [SerializeField, BoxGroup("References")] protected ZombieAnimationEventReceiver m_ZombieAnimationEventReceiver;
    [SerializeField, BoxGroup("References")] protected CharacterController m_CharacterController;
    [SerializeField, BoxGroup("References")] protected HealthBar m_HealthBar;
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected LegsAnimator m_LegsAnimator;
    [SerializeField, BoxGroup("References")] protected MeshRenderer m_HealthBarMesh;
    [SerializeField, BoxGroup("Data")] protected ZombieAnimationSO m_AnimationKeySO;
    [ShowInInspector, ReadOnly] protected EnemyStats m_EnemyStats;
    protected bool m_IsAlive = true;
    protected bool m_IsActive = false;
    protected bool m_IsLocal = false;
    [SerializeField] protected EnemyStatsSO m_StatsSO;

    public virtual void Init(EnemyStatsSO statsSO = null)
    {
        m_StatsSO = statsSO;
        m_EnemyStats = new EnemyStats();
        m_EnemyStats.LoadStats(m_StatsSO);
        m_ZombieAnimationEventReceiver.Init(this);
        if (m_ZombieAnimationEventReceiver == null && m_Animator != null)
            m_ZombieAnimationEventReceiver = m_Animator.GetComponent<ZombieAnimationEventReceiver>();
    }

#if UNITY_EDITOR
    [Button]
    public virtual void InitDataEditor(EnemyStatsSO statsSO = null)
    {
        m_StatsSO = statsSO;
        m_ZombieAnimationEventReceiver.Init(this);
        if (m_ZombieAnimationEventReceiver == null && m_Animator != null)
            m_ZombieAnimationEventReceiver = m_Animator.GetComponent<ZombieAnimationEventReceiver>();
    }

    [Button]
    private void Push()
    {
        m_LegsAnimator.User_AddImpulse(m_BlockHit);
    }

    #endif
}
