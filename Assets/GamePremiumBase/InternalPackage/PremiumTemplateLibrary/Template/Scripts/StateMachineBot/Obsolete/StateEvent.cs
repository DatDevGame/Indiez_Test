using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Premium.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use Premium.StateMachine instead")]
    public class StateEvent : MonoBehaviour
    {
        public event Action Triggered = delegate {};
        protected void Trigger(){
            Triggered();
        }
        public virtual void EnableTrigger(){}
        public virtual void DisableTrigger(){}
    }
}