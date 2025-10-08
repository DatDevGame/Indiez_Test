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
using Random = UnityEngine.Random;

[Serializable]
public class AttackingTargetState : AIBotState
{
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnStateEnable()
    {
        base.OnStateEnable();
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (BotController.Target == null)
            return;

        // float distanceToTarget = Vector3.Distance(BotController.BotTransform.position, BotController.Target.GetTargetPoint());
        // if (distanceToTarget > BotController.AIProfile?.AttackRange)
        // {
        //     // Target is out of attack range, stop attacking
        // }
    }

    private void PerformAttack()
    {
        Debug.Log($"AI Bot attacking target at {BotController.Target.GetTargetPoint()}");
    }

    public override void InitializeState(AIBotController botController)
    {
        base.InitializeState(botController);
    }
}

[Serializable]
public class AttackingTargetToChasingTargetTransition : AIBotStateTransition
{
    private AttackingTargetState attackingTargetState;
    protected override bool Decide()
    {
        //Condition Transition In Here
        // if (botController.Target == null)
        //     return false;

        // float distanceToTarget = Vector3.Distance(botController.BotTransform.position, botController.Target.GetTargetPoint());
        // bool inAttackRange = distanceToTarget <= botController.AIProfile?.AttackRange;
        // bool inDetectionRange = distanceToTarget <= botController.AIProfile?.DetectionRange;

        // if (!inAttackRange && inDetectionRange)
        // {
        //     Debug.Log($"AttackingTarget->ChasingTarget because target moved out of attack range (distance: {distanceToTarget:F2})");
        //     return true;
        // }
        return false;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        base.InitializeTransition(originState, botController);
        attackingTargetState = GetOriginStateAsType<AttackingTargetState>();
    }
}