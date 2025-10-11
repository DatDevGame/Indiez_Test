using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ZombieLookingTargetState : AIBotState
{
    protected ZombieAIController m_ZombieAIController;
    public INavigationPoint TargetSearch;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnStateEnable()
    {
        m_ZombieAIController.NavMeshAgent.isStopped = true;
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Walking, false);
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Idle, true);
    }

    protected override void OnStateUpdate()
    {
        if (m_ZombieAIController != null)
        {
            if (!m_ZombieAIController.IsAvailable())
                return;

            List<INavigationPoint> navigationPoints = m_ZombieAIController.FindTargetsInRange();
            if (navigationPoints.Count > 0)
            {
                m_ZombieAIController.Target = navigationPoints
                    .Where(v => v != null && v.IsAvailable())
                    .OrderBy(point => Vector3.Distance(m_ZombieAIController.transform.position, point.GetSelfPoint()))
                    .FirstOrDefault();
            }
        }
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeState(botController);
        m_ZombieAIController.Animator.SetTrigger(m_ZombieAIController.AnimationKeySO.Idle);
    }
}

[Serializable]
public class ZombieLookingTargetToChasingTargetTransition : AIBotStateTransition
{
    protected ZombieAIController m_ZombieAIController;
    protected ZombieLookingTargetState m_LookingState;
    protected float m_TriggerTimer;
    protected float m_InitialTriggerTime;

    protected override bool Decide()
    {
        if (!m_ZombieAIController.Target.IsAvailable())
            return false;

        if (m_ZombieAIController.Target != null & m_ZombieAIController.IsAvailable())
        {
            m_TriggerTimer -= Time.deltaTime;
            float timeLookAt = m_InitialTriggerTime * 0.5f;
            if (m_TriggerTimer <= timeLookAt)
                m_ZombieAIController.BotTransform.DOLookAt(m_ZombieAIController.GetTargetPoint(), timeLookAt * 0.5f);

            if (m_TriggerTimer <= 0)
            {
                m_InitialTriggerTime = m_ZombieAIController.ZombieAIProfile.TimeRandomChasingTarget;
                m_TriggerTimer = m_InitialTriggerTime;
                return true;
            }
        }
        else
        {
            m_TriggerTimer = m_ZombieAIController.ZombieAIProfile.TimeRandomChasingTarget;
        }
        return false;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeTransition(originState, botController);
        m_LookingState = GetOriginStateAsType<ZombieLookingTargetState>();
        m_InitialTriggerTime = m_ZombieAIController.ZombieAIProfile.TimeRandomChasingTarget;
        m_TriggerTimer = m_InitialTriggerTime;
    }
}
