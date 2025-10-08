using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCore.Helpers;
using HCore.Events;
using Premium;
using Premium.StateMachine;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIBotController : MonoBehaviour
{
    public event Action<INavigationPoint> OnTargetReached;
    public event Action<INavigationPoint> OnTargetChanged;

    [Header("AI Configuration")]
    [SerializeField] protected AIProfile m_AIProfile;
    [SerializeField] protected NavMeshAgent m_NavMeshAgent;
    [SerializeField] protected Transform m_BotTransform;

    [Header("State Machine")]
    [SerializeReference] protected List<AIBotState> m_States;


    protected bool m_IsRunning = false;
    protected INavigationPoint m_Target;
    protected StateMachine.Controller m_StateMachineController = new StateMachine.Controller();
    protected float m_LastAttackTime;
    protected float m_TargetLostTime;


    public StateMachine.Controller StateMachineController => m_StateMachineController;
    public AIBotState CurrentState => m_StateMachineController.CurrentState as AIBotState;
    public AIProfile AIProfile => m_AIProfile;
    public NavMeshAgent NavMeshAgent => m_NavMeshAgent;
    public Transform BotTransform => m_BotTransform;
    public List<AIBotState> StatesInfos => m_States;

    public INavigationPoint Target
    {
        get => m_Target;
        set
        {
            var previousTarget = m_Target;
            m_Target = value;
            if (previousTarget != m_Target)
            {
                OnTargetChanged?.Invoke(m_Target);
                if (m_Target == null)
                {
                    m_TargetLostTime = Time.time;
                }
            }
        }
    }

    protected virtual void Awake()
    {
        // Auto-assign components if not set
        if (m_NavMeshAgent == null)
            m_NavMeshAgent = GetComponent<NavMeshAgent>();

        if (m_BotTransform == null)
            m_BotTransform = transform;

        // Validate AI Profile
        if (m_AIProfile == null)
        {
            Debug.LogError($"AIProfile is not assigned to {gameObject.name}!");
        }
    }

    protected virtual void Start()
    {
        InitializeStateMachine();
    }

    protected virtual void Update()
    {
        if (!m_IsRunning) return;
        m_StateMachineController.Update();
    }

    public virtual void InitializeStateMachine()
    {
        foreach (var state in m_States)
        {
            state.InitializeState(this);
        }
        StartStateMachine();
    }

    public virtual void StartStateMachine()
    {
        if (m_States.Count <= 0)
            return;
        m_IsRunning = true;
        m_StateMachineController.StateChanged(m_States[0]);
    }

    public virtual void StopStateMachine()
    {
        m_IsRunning = false;
        m_StateMachineController.Stop();
        StopAllCoroutines();
    }

    public virtual AIBotState FindStateById(string stateId)
    {
        return FindStateById<AIBotState>(stateId);
    }

    public T FindStateById<T>(string stateId) where T : AIBotState
    {
        foreach (var state in m_States)
        {
            if (state.StateId == stateId)
                return state as T;
        }
        return null;
    }

    public virtual void Initialize(AIBotController botController)
    {
        if (m_AIProfile != null && m_NavMeshAgent != null)
        {
            // m_NavMeshAgent.speed = m_AIProfile.MoveSpeed;
            // m_NavMeshAgent.angularSpeed = m_AIProfile.RotationSpeed * 100f;
            // m_NavMeshAgent.stoppingDistance = m_AIProfile.ReachThreshold;
        }
    }

    public virtual List<INavigationPoint> FindTargetsInRange()
    {
        // var targets = new List<INavigationPoint>();
        // if (m_AIProfile == null)
        //     return targets;

        // var colliders = Physics.OverlapSphere(m_BotTransform.position, m_AIProfile.DetectionRange, m_AIProfile.TargetLayerMask);
        // foreach (var collider in colliders)
        // {
        //     var navPoint = collider.GetComponent<INavigationPoint>();
        //     if (navPoint != null && navPoint.IsAvailable())
        //     {
        //         targets.Add(navPoint);
        //     }
        // }
        // return targets;
        return null;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // if (m_AIProfile != null && m_AIProfile.ShowDebugGizmos)
        // {
        //     // Draw detection range
        //     Gizmos.color = m_AIProfile.DebugColor;
        //     Gizmos.DrawWireSphere(m_BotTransform.position, m_AIProfile.DetectionRange);

        //     // Draw attack range
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(m_BotTransform.position, m_AIProfile.AttackRange);

        //     // Draw current target if exists
        //     if (m_Target != null)
        //     {
        //         Gizmos.color = Color.yellow;
        //         Gizmos.DrawLine(m_BotTransform.position, m_Target.GetTargetPoint());
        //     }
        // }
    }
}