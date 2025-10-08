using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Premium;

namespace Premium
{
    public class EZAnimVector2 : EZAnimUnityEvent<Vector2>
    {
        protected override void SetAnimationCallBack()
        {
            AnimationCallBack = t =>
            {
                UnityAnimationCallBack?.Invoke(Vector2.LerpUnclamped(from, to, t));
            };
            base.SetAnimationCallBack();
        }
    }
}