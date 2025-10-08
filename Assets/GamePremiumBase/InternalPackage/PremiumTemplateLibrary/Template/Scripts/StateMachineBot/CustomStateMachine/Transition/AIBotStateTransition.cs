using System;
using System.Collections;
using System.Collections.Generic;
using Premium.StateMachine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using UnityEngine.Assertions.Must;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public abstract class AIBotStateTransition
{
    [SerializeField, ReadOnly]
    [HorizontalGroup("StateIDRow")]
    protected string targetStateID;

    protected AIBotState originState;
    protected AIBotController botController;
    protected StateMachine.State.Transition transition;

    protected abstract bool Decide();

    protected virtual StateMachine.Event GetEventTrigger() => new AIBotTransitionEvent(Decide, botController, this);

    public virtual T GetOriginStateAsType<T>() where T : AIBotState
    {
        return originState as T;
    }

    public virtual T GetTargetStateAsType<T>() where T : AIBotState
    {
        return GetTransition().TargetState as T;
    }

    public virtual StateMachine.State.Transition GetTransition()
    {
        return transition;
    }

    public virtual void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        var targetState = botController.FindStateById(targetStateID) as StateMachine.State;
        this.originState = originState;
        this.botController = botController;
        this.transition = new StateMachine.State.Transition(GetEventTrigger(), targetState);
    }


#if UNITY_EDITOR
    [HorizontalGroup("StateIDRow")]
    [Button("Path Target StateID -> Transition", ButtonSizes.Small)]
    private void PasteTargetStateID()
    {
        targetStateID = EditorGUIUtility.systemCopyBuffer;
        Debug.Log($"Successfully Path TargetStateID: {targetStateID}");
    }
#endif
}
public class AIBotTransitionEvent : StateMachine.PredicateEvent
{
    public AIBotTransitionEvent(Func<bool> predicate, AIBotController botController, AIBotStateTransition transition) : base(predicate)
    {
        this.botController = botController;
        this.transition = transition;
    }

    private AIBotController botController;
    private AIBotStateTransition transition;

    public override void Update()
    {
        if (botController.CurrentState != transition.GetOriginStateAsType<AIBotState>())
            return;
        base.Update();
    }
}