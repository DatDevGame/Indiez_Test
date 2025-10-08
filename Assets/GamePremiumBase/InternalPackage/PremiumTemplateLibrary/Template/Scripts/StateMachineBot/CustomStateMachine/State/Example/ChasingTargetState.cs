using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class ChasingTargetState : AIBotState
{
    private static int StaticGroundLayer;
    private static int DynamicGroundLayer;
    private static readonly Color[] GizmosColors = new Color[] { Color.red, Color.green, Color.blue };

    private bool m_HasAnyDynamicGround;
    private Vector3 m_TargetPosition;
    private Vector3 m_LastTargetPosition = Vector3.negativeInfinity;
    private NavMeshPath m_NavMeshPath;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnStateEnable()
    {
        base.OnStateEnable();
    }

    protected override void OnStateDisable()
    {
        base.OnStateDisable();
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        if (BotController.Target == null)
            return;
        m_TargetPosition = BotController.Target.GetTargetPoint();
    }

    public override void InitializeState(AIBotController botController)
    {
        base.InitializeState(botController);
        m_NavMeshPath = new NavMeshPath();
    }

    public bool IsReachTarget()
    {
        return false;
    }

    public bool IsOnDynamicGround(Vector3 wayPoint)
    {
        if (!m_HasAnyDynamicGround)
            return false;
        if (!NavMesh.SamplePosition(wayPoint, out NavMeshHit dynamicGroundHit, 5f, DynamicGroundLayer))
            return false;
        if (!NavMesh.SamplePosition(wayPoint, out NavMeshHit staticGroundHit, 5f, StaticGroundLayer))
            return true;
        return dynamicGroundHit.distance < staticGroundHit.distance;
    }

}

[Serializable]
public class ChasingTargetToAttackingTargetTransition : AIBotStateTransition
{
    private ChasingTargetState chasingTargetState;
    protected override bool Decide()
    {
        //Condition Transition In Here
        return false;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        base.InitializeTransition(originState, botController);
        chasingTargetState = GetOriginStateAsType<ChasingTargetState>();
    }
}