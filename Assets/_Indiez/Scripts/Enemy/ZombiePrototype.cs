using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ZombiePrototype : EnemyBase, IDamageable
{
    [SerializeField, BoxGroup("Resource")] protected HealthBarSO m_HealthBarSO;

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

    public void TakeDamage(float amount)
    {
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
        TakeDamage(amout);
    }
#endif
}
