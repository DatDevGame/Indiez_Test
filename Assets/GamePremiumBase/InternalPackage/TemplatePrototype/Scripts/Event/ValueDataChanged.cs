using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCore.Events
{
    public struct ValueDataChanged<T>
    {
        public T oldValue { get; set; }
        public T newValue { get; set; }

        public ValueDataChanged(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}