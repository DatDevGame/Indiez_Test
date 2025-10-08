using UnityEngine;

namespace Premium.PopupManagement
{
    public class ManualTriggerPopup : IPopup
    {
        public int priority { get; set; }

        private SubscriptionEvent onStartShow { get; set; } = new SubscriptionEvent();
        private SubscriptionEvent onEndShow { get; set; } = new SubscriptionEvent();
        private SubscriptionEvent onStartHide { get; set; } = new SubscriptionEvent();
        private SubscriptionEvent onEndHide { get; set; } = new SubscriptionEvent();

        public int GetPriorityOrder()
        {
            return priority;
        }

        public void Show()
        {
            ShowImmediately();
        }

        public void ShowImmediately()
        {
            GetOnStartShowEvent().Invoke();
            GetOnEndShowEvent().Invoke();
        }

        public void Hide()
        {
            HideImmediately();
        }

        public void HideImmediately()
        {
            GetOnStartHideEvent().Invoke();
            GetOnEndHideEvent().Invoke();
        }

        public SubscriptionEvent GetOnStartShowEvent()
        {
            return onStartShow;
        }

        public SubscriptionEvent GetOnEndShowEvent()
        {
            return onEndShow;
        }

        public SubscriptionEvent GetOnStartHideEvent()
        {
            return onStartHide;
        }

        public SubscriptionEvent GetOnEndHideEvent()
        {
            return onEndHide;
        }
    }
}