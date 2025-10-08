using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Premium.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use Premium.StateMachine instead")]
    public class EnterTrigger : StateEvent
    {
        [SerializeField] private TriggerEvent trigger = null;
        [SerializeField] private string tagCompare = "";

        private void Awake() {
            trigger.OnTriggerEnteredEvent += TriggerEntered;
        }

        private void OnDestroy() {
            trigger.OnTriggerEnteredEvent -= TriggerEntered;
        }

        private void TriggerEntered(Collider obj)
        {
            if(!string.IsNullOrEmpty(tagCompare))
            {
                if(tagCompare != obj.tag)
                    return;
            }
            Trigger();
        }
    }
}