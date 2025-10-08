using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Premium.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use Premium.StateMachine instead")]
    public abstract class StateBehaviour : MonoBehaviour
    {
        public abstract void UpdateBehaviour();
        public virtual void OnBehaviourEnable(){}
        public virtual void OnBehaviourDisable(){}
    }
}