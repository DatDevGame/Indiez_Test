using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HCore.Helpers;
using Premium;
using Premium.StateMachine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class ZombieAttackingTargetState : AIBotState
{
    protected ZombieAIController m_ZombieAIController;
    protected float m_TriggerTimer;
    protected float m_ForwardDistance = 0.8f;
    protected const float LOOK_AT_DURATION = 0.2F;
    protected IDamageable m_Target;

    protected override void OnStateDisable()
    {
        m_TriggerTimer = 0;
    }
    protected override void OnStateUpdate()
    {
        if (m_ZombieAIController.Target == null)
            return;

        float distanceTarget = Vector3.Distance(m_ZombieAIController.BotTransform.position, m_ZombieAIController.GetTargetPoint());
        bool isTooClose = distanceTarget < m_ZombieAIController.EnemyBase.EnemyStats?.AttackRange * 0.95f;

        if (isTooClose)
        {
            Vector3 dirAway = (m_ZombieAIController.BotTransform.position - m_ZombieAIController.GetTargetPoint());
            dirAway.y = 0f;
            dirAway.Normalize();

            float retreatSpeed = 0.8f;
            Vector3 retreatVelocity = dirAway * retreatSpeed;

            if (retreatVelocity.magnitude > 1f)
                retreatVelocity = retreatVelocity.normalized;

            if (m_ZombieAIController.NavMeshAgent.enabled)
                m_ZombieAIController.NavMeshAgent.enabled = false;

            m_ZombieAIController.CharacterController.Move(retreatVelocity * Time.deltaTime);
        }
        else
        {
            if (!m_ZombieAIController.NavMeshAgent.enabled)
                m_ZombieAIController.NavMeshAgent.enabled = true;
        }

        Debug.Log($"Attack State - {isTooClose}");

        m_ZombieAIController.BotTransform.DOLookAt(m_ZombieAIController.GetTargetPoint(), m_ZombieAIController.EnemyBase.StatsSOData.LookAtDuration);
        m_TriggerTimer -= Time.deltaTime;
        if (m_TriggerTimer <= 0 && m_ZombieAIController.Target.IsAvailable())
            PerformAttack();
    }

    private void PerformAttack()
    {
        m_ZombieAIController.Animator.SetBool(m_ZombieAIController.AnimationKeySO.Walking, false);
        m_ZombieAIController.Animator.SetTrigger(m_ZombieAIController.AnimationKeySO.Attack);

        float animationLength = 0f;
        AnimatorStateInfo stateInfo = m_ZombieAIController.Animator.GetCurrentAnimatorStateInfo(0);
        animationLength = stateInfo.length / 1.5f;

        m_TriggerTimer = m_ZombieAIController.EnemyBase.EnemyStats.AttackCoolDown + animationLength;
        LayerMask targetLayer = m_ZombieAIController.EnemyBase.EnemyStats.TargetLayermask;

        Vector3 origin = m_ZombieAIController.transform.position + Vector3.up * 0.5f;
        Vector3 direction = m_ZombieAIController.transform.forward * m_ForwardDistance;
        float attackRange = m_ForwardDistance;

#if UNITY_EDITOR
        Debug.DrawLine(origin, origin + direction * attackRange, Color.cyan, 99);
#endif

        Debug.Log($"Handle Attack Pro 1 - {targetLayer}");
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 99, targetLayer))
        {
            Debug.Log($"Handle Attack Pro 2.0");
            Debug.Log($"Handle Attack Pro 2 -{hit.collider.name}");
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
                m_Target = target;
        }
    }

    //Call In Animation
    public void HandleAttackHit()
    {
        float distanceAttack = Vector3.Distance(m_ZombieAIController.transform.position, m_ZombieAIController.GetTargetPoint());
        Debug.Log($"Handle Attack - {distanceAttack} <= {m_ZombieAIController.EnemyBase.EnemyStats.AttackRange}");
        if (m_Target == null)
            Debug.Log($"Handle Attack 2");
        if (distanceAttack <= m_ZombieAIController.EnemyBase.EnemyStats.AttackRange && m_Target != null)
        {
            m_Target.TakeDamage(m_ZombieAIController.EnemyBase.EnemyStats.AttackDamage);
            //SoundManager.Instance.PlayLoopSFX(m_ZombieAIController.EnemyBase.GetRandomPunchSound(), volumn: 0.5f);
            Debug.Log($"Handle Attack - 3");
        }

    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeState(botController);

        if (m_ZombieAIController != null)
            m_ZombieAIController.EnemyBase.ZombieAnimationEventReceiver.SetAttackingState(this);
        Debug.Log($"InitializeState -> Zombie Attacking State");
    }
}

[Serializable]
public class ZombieAttackingToLookTransition : AIBotStateTransition
{
    protected ZombieAIController m_ZombieAIController;
    protected ZombieAttackingTargetState m_AttackingTargetState;
    protected override bool Decide()
    {
        //Condition Transition In Here
        if (m_ZombieAIController.Target == null)
            return false;
        if (!m_ZombieAIController.Target.IsAvailable())
            return true;

        float distanceToTarget = Vector3.Distance(m_ZombieAIController.BotTransform.position, m_ZombieAIController.Target.GetSelfPoint());
        bool isOutRangeAttack = distanceToTarget > m_ZombieAIController.EnemyBase.EnemyStats.AttackRange;
        return isOutRangeAttack;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is ZombieAIController zombieAIController)
            m_ZombieAIController = zombieAIController;
        base.InitializeTransition(originState, botController);
        m_AttackingTargetState = GetOriginStateAsType<ZombieAttackingTargetState>();
        Debug.Log($"InitializeTransition -> Zombie Attacking To Look Transition");
    }
}