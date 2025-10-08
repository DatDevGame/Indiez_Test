using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Premium.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public abstract class AIBotState : StateMachine.State
{
    protected AIBotState() : base(null)
    {
    }

#if UNITY_EDITOR
    [InfoBox("StateId is empty or whitespace!, Please Create ID", InfoMessageType.Error, "@string.IsNullOrWhiteSpace(stateId)")]
#endif
    [HorizontalGroup("StateIDRow")]
    [SerializeField, ReadOnly, LabelText("State ID")]
    [HideLabel]
    protected string stateId;

    [SerializeReference, PropertyOrder(10)]
    protected List<AIBotStateTransition> transitions;
    protected AIProfile AIProfile => BotController.AIProfile;
    public string StateId => stateId;
    public AIBotController BotController { get; protected set; }

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(stateId))
            stateId = Guid.NewGuid().ToString();
    }

    protected virtual void OnDrawGizmos()
    {

    }

    protected virtual void OnDrawGizmosSelected()
    {

    }

    protected virtual void OnStateEnable()
    {

    }

    protected virtual void OnStateDisable()
    {

    }

    protected virtual void OnStateUpdate()
    {

    }

    public virtual void InitializeState(AIBotController botController)
    {
        foreach (var transition in transitions)
        {
            transition.InitializeTransition(this, botController);
        }

        BotController = botController;
        Transitions = transitions.Select(transition => transition.GetTransition()).ToList();
        Behaviour = new StateMachine.ActionBehaviour(OnStateEnable, OnStateDisable, OnStateUpdate);
    }

#if UNITY_EDITOR
    [HorizontalGroup("StateIDRow")]
    [Button("Create ID", ButtonSizes.Small), Tooltip("Generate new State ID if it's empty or invalid")]
    private void CreateID()
    {
        if (string.IsNullOrWhiteSpace(stateId))
        {
            Debug.LogWarning("⚠️ stateId is null or contains only whitespace! Generating a new one automatically.");
            stateId = Guid.NewGuid().ToString();
        }
        else
        {
            Debug.Log($"✅ stateId is valid: {stateId}");
        }
    }

    [HorizontalGroup("StateIDRow")]
    [Button("Copy ID", ButtonSizes.Small)]
    private void CopyStateID()
    {
        EditorGUIUtility.systemCopyBuffer = stateId;
        Debug.Log($"Successfully copied stateId: {stateId}");
    }
#endif
}