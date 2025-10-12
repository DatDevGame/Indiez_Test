using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HCore.Helpers;
using Premium;
using Premium.StateMachine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class ZombieChasingTargetState : AIBotState
{
    protected ZombieAIController m_ZombieAIController;
    protected Vector3 m_LastTargetPosition;
    protected float m_RepathThreshold = 0.2f;
    protected float m_ForceRepathInterval = 1f;
    protected float m_ForceRepathTimer = 0f;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnStateEnable()
    {
        if (m_ZombieAIController.NavMeshAgent != null)
        {
            m_ZombieAIController.NavMeshAgent.speed = m_ZombieAIController.EnemyBase.StatsSOData.MoveSpeed;
            m_ZombieAIController.NavMeshAgent.angularSpeed = m_ZombieAIController.ZombieAIProfile.RotationSpeed * 100f;
            m_ZombieAIController.NavMeshAgent.stoppingDistance = m_ZombieAIController.ZombieAIProfile.ReachThreshold;

            if (!m_ZombieAIController.NavMeshAgent.enabled)
                m_ZombieAIController.NavMeshAgent.enabled = true;
        }
        m_ZombieAIController.NavMeshAgent.isStopped = false;
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Idle, false);
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Walking, true);
    }

    protected override void OnStateDisable()
    {
        if (m_ZombieAIController.NavMeshAgent.enabled)
            m_ZombieAIController.NavMeshAgent.isStopped = true;
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        if (m_ZombieAIController.Target == null)
            return;

        MoveTarget(m_ZombieAIController.GetTargetPoint(), Time.deltaTime);
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeState(botController);
    }

    protected virtual void MoveTarget(Vector3 targetPosition, float deltaTime)
    {
        if (!IsVectorValid(targetPosition))
            return;

        m_ZombieAIController.Visual.transform.DOLookAt(targetPosition, 0.2f, AxisConstraint.Y);

        m_ForceRepathTimer += deltaTime;

        bool shouldMove = m_LastTargetPosition != targetPosition || m_ForceRepathTimer >= m_ForceRepathInterval;

        if (shouldMove && m_ZombieAIController.Target.IsAvailable())
        {
            m_LastTargetPosition = targetPosition;
            if (m_ZombieAIController.NavMeshAgent.isOnNavMesh)
                m_ZombieAIController.NavMeshAgent.SetDestination(targetPosition);

            m_ForceRepathTimer = 0f;
        }
    }

    private bool IsVectorValid(Vector3 vec)
    {
        return !(float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z)
              || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z));
    }

}

[Serializable]
public class ZombieChasingToAttackTransition : AIBotStateTransition
{
    protected ZombieAIController m_ZombieAIController;
    private ZombieChasingTargetState m_ZombieChasingTargetState;

    protected override bool Decide()
    {
        if (!m_ZombieAIController.Target.IsAvailable())
            return false;
        return CheckAttackRange(m_ZombieAIController.GetTargetPoint());
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeTransition(originState, botController);
        m_ZombieChasingTargetState = GetOriginStateAsType<ZombieChasingTargetState>();
    }

    protected bool CheckAttackRange(Vector3 vecTarget)
    {
        if (m_ZombieAIController == null) return false;
        float distanceToTarget = Vector3.Distance(m_ZombieAIController.transform.position, vecTarget);
        return distanceToTarget <= m_ZombieAIController.EnemyBase.EnemyStats.AttackRange && m_ZombieAIController.IsAvailable();
    }
}
