using UnityEngine;

namespace Premium.EditableStateMachine
{
    using Premium.StateMachine;

    public abstract class TransitionSO : ScriptableObject
    {
        [SerializeField] protected StateSO targetState;

        protected StateMachine.State.Transition transition;

        public virtual StateMachine.State.Transition Transition
        {
            get
            {
                return transition;
            }
        }

        public abstract void SetupTransition(object[] parameters);
    }

    public abstract class StateTransitionParams
    {

    }
}