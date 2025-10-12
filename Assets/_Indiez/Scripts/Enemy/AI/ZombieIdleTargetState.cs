using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ZombieIdleTargetState : AIBotState
{
    protected ZombieAIController m_ZombieAIController;

    protected override void OnStateEnable()
    {
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Walking, false);
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Idle, true);
    }
    public override void InitializeState(AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeState(botController);
    }
}

[Serializable]
public class ZombieIdleTargetToLookingTargetTransition : AIBotStateTransition
{
    protected ZombieAIController m_ZombieAIController;
    protected ZombieIdleTargetState m_IdleTargetState;
    protected float m_SearchDelay;
    protected override bool Decide()
    {
        if (!m_ZombieAIController.EnemyBase.IsAlive)
            return false;
        return true;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeTransition(originState, botController);
        m_IdleTargetState = GetOriginStateAsType<ZombieIdleTargetState>();
    }
}