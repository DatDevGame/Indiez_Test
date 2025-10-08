# AIBotController Transition System - Quick Start Guide

## What are Transitions?
Transitions automatically switch your AI bot between different states (like Looking → Chasing → Attacking) based on conditions you define.

## How to Use Transitions in 5 Steps:

### Step 1: Create a Transition Class
```csharp
[Serializable]
public class MyTransition : AIBotStateTransition
{
    protected override bool Decide()
    {
        // Return true when you want to switch states
        return botController.Target != null;
    }
}
```

### Step 2: Add Transitions to Your State
In your state class, add transitions to the list:
```csharp
[SerializeField] private List<AIBotStateTransition> m_Transitions;
```

### Step 3: Set Target State ID
In Unity Inspector:
- Select your state
- In the transitions list, set `targetStateID` to the ID of the state you want to go to

### Step 4: Initialize the State Machine
Make sure your AIBotController calls:
```csharp
InitializeStateMachine();
```

### Step 5: Start the State Machine
```csharp
StartStateMachine();
```

## Common Transition Examples:

### Target Found Transition
```csharp
protected override bool Decide()
{
    return botController.Target != null;
}
```

### Distance-Based Transition
```csharp
protected override bool Decide()
{
    if (botController.Target == null) return false;
    
    float distance = Vector3.Distance(
        botController.BotTransform.position, 
        botController.Target.GetTargetPoint()
    );
    
    return distance <= 5f; // Switch when within 5 units
}
```

### Time-Based Transition
```csharp
private float m_StartTime;

public override void InitializeTransition(AIBotState originState, AIBotController botController)
{
    base.InitializeTransition(originState, botController);
    m_StartTime = Time.time;
}

protected override bool Decide()
{
    return Time.time - m_StartTime >= 3f; // Switch after 3 seconds
}
```

## Troubleshooting:
- **Transitions not working?** Check that `targetStateID` is set correctly
- **State not switching?** Make sure the state machine is running (`m_IsRunning = true`)
- **Performance issues?** Keep `Decide()` methods lightweight

## Key Points:
1. Only implement the `Decide()` method - return `true` when you want to transition
2. Set the `targetStateID` in the inspector
3. The system automatically checks conditions every frame
4. Transitions only work when the origin state is the current state 