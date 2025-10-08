using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// COMPREHENSIVE GUIDE: How to Use AIBotStateTransition System
/// 
/// This system allows you to create state transitions for your AI bot.
/// Transitions automatically check conditions and switch between states.
/// </summary>

// ============================================================================
// STEP 1: CREATE A TRANSITION CLASS
// ============================================================================
[Serializable]
public class ExampleTransition : AIBotStateTransition
{
    // This is the ONLY method you need to implement!
    // Return true when you want to transition to the target state
    protected override bool Decide()
    {
        // Example: Transition when target is found
        if (botController.Target != null)
        {
            Debug.Log("Transitioning because target was found!");
            return true;
        }
        
        // Example: Transition after a time delay
        // if (Time.time > someTimeThreshold)
        // {
        //     return true;
        // }
        
        return false;
    }
    
    // Optional: Override InitializeTransition if you need custom setup
    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        base.InitializeTransition(originState, botController);
        // Custom initialization code here
    }
    
    // Optional: Get the origin state as a specific type
    public ExampleState GetOriginState()
    {
        return GetOriginStateAsType<ExampleState>();
    }
    
    // // Optional: Get the target state as a specific type
    // public TargetState GetTargetState()
    // {
    //     return GetTargetStateAsType<TargetState>();
    // }
}

// ============================================================================
// STEP 2: CREATE YOUR STATE CLASS
// ============================================================================
[Serializable]
public class ExampleState : AIBotState
{
    // Add your transitions here - they will be automatically initialized
    [SerializeField] private List<AIBotStateTransition> m_Transitions;
    
    protected override void OnStateUpdate()
    {
        // Your state logic here
        Debug.Log("ExampleState is running...");
        
        // The transitions will automatically check their conditions
        // and switch states when Decide() returns true
    }
    
    public override void InitializeState(AIBotController botController)
    {
        // Make sure to call base to initialize transitions
        base.InitializeState(botController);
        
        // Custom initialization code here
    }
}

// ============================================================================
// STEP 3: COMMON TRANSITION PATTERNS
// ============================================================================

// Pattern 1: Target-based transitions
[Serializable]
public class HasTargetTransition : AIBotStateTransition
{
    protected override bool Decide()
    {
        return botController.Target != null;
    }
}

[Serializable]
public class NoTargetTransition : AIBotStateTransition
{
    protected override bool Decide()
    {
        return botController.Target == null;
    }
}

// Pattern 2: Distance-based transitions
[Serializable]
public class TargetInRangeTransition : AIBotStateTransition
{
    [SerializeField] private float m_Range = 5f;
    
    protected override bool Decide()
    {
        if (botController.Target == null) return false;
        
        float distance = Vector3.Distance(
            botController.BotTransform.position, 
            botController.Target.GetTargetPoint()
        );
        
        return distance <= m_Range;
    }
}

// Pattern 3: Time-based transitions
[Serializable]
public class TimeElapsedTransition : AIBotStateTransition
{
    [SerializeField] private float m_Duration = 3f;
    private float m_StartTime;
    
    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        base.InitializeTransition(originState, botController);
        m_StartTime = Time.time;
    }
    
    protected override bool Decide()
    {
        return Time.time - m_StartTime >= m_Duration;
    }
}

// Pattern 4: Health-based transitions
[Serializable]
public class LowHealthTransition : AIBotStateTransition
{
    [SerializeField] private float m_HealthThreshold = 0.3f;
    
    protected override bool Decide()
    {
        // Assuming you have a health component
        // var health = botController.GetComponent<HealthComponent>();
        // return health.CurrentHealth / health.MaxHealth <= m_HealthThreshold;
        return false; // Placeholder
    }
}

// Pattern 5: Custom condition transitions
[Serializable]
public class CustomConditionTransition : AIBotStateTransition
{
    [SerializeField] private bool m_SomeCondition;
    
    protected override bool Decide()
    {
        // Check your custom condition
        return m_SomeCondition;
    }
}

// ============================================================================
// STEP 4: HOW TO SETUP IN UNITY INSPECTOR
// ============================================================================
/*
1. In your AIBotController, add states to the m_States list
2. For each state, in the inspector:
   - Add transitions to the transitions list
   - Set the targetStateID to the ID of the state you want to transition to
3. The system will automatically:
   - Initialize all transitions
   - Check conditions every frame
   - Switch states when conditions are met
*/

// ============================================================================
// STEP 5: COMPLETE EXAMPLE
// ============================================================================
[Serializable]
public class PatrolState : AIBotState
{
    protected override void OnStateUpdate()
    {
        // Patrol logic here
        Debug.Log("Patrolling...");
    }
}

[Serializable]
public class ChaseState : AIBotState
{
    protected override void OnStateUpdate()
    {
        // Chase logic here
        // if (botController.Target != null)
        // {
        //     botController.MoveToTarget(botController.Target.GetTargetPoint());
        // }
    }
}

[Serializable]
public class PatrolToChaseTransition : AIBotStateTransition
{
    protected override bool Decide()
    {
        // Transition when we find a target
        return botController.Target != null;
    }
}

[Serializable]
public class ChaseToPatrolTransition : AIBotStateTransition
{
    protected override bool Decide()
    {
        // Transition when we lose the target
        return botController.Target == null;
    }
}

// ============================================================================
// TROUBLESHOOTING
// ============================================================================
/*
Common Issues:
1. Transitions not working?
   - Make sure targetStateID is set correctly
   - Check that Decide() returns true when you expect it to
   - Verify the state machine is running (m_IsRunning = true)

2. State not switching?
   - Ensure the origin state is the current state
   - Check that the target state exists in the state machine
   - Verify transition conditions are being met

3. Performance issues?
   - Keep Decide() methods lightweight
   - Avoid expensive operations in transition checks
   - Consider using cooldowns for frequent checks
*/ 