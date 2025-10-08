using System;
using System.Collections;
using System.Collections.Generic;
using Premium;
using UnityEngine;

namespace Premium.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use Premium.StateMachine instead")]
    public class WaitTrigger : StateEvent
    {
        [SerializeField] private float time = 1;
        public override void EnableTrigger()
        {
            base.EnableTrigger();
            StopAllCoroutines();
            StartCoroutine(CommonCoroutine.Delay(time, false, ()=>Trigger()));
        }
    }
}