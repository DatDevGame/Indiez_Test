
using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Premium.PopupManagement
{
    [CreateAssetMenu(fileName = "PopupSO", menuName = "Premium/Popup/PopupSO")]
    public class PopupSO : SerializableScriptableObject, IPopup
    {
        [SerializeField]
        protected bool m_IsShowImmediately = false;
        [SerializeField]
        protected bool m_OverrideCanvasSortingOrder = true; // Whether to override the canvas sorting
        [SerializeField, MaxValue(short.MaxValue)]
        protected int m_Priority = 10; // Priority level of this popup (used for sorting/display order)
        [SerializeField]
        protected Popup m_PopupPrefab; // Assign the UI prefab for this popup
        [NonSerialized, ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        protected IPopup m_PopupInstance; // The runtime popup instance

        public virtual bool isShowImmediately => m_IsShowImmediately;
        public virtual bool overrideCanvasSortingOrder => m_OverrideCanvasSortingOrder;
        public virtual IPopup popup
        {
            get
            {
                if (IsPopupNull())
                {
                    m_PopupInstance = Instantiate(m_PopupPrefab).GetComponent<IPopup>();
                }
                return m_PopupInstance;
            }
            set
            {
                m_PopupInstance = value;
            }
        }

#if UNITY_EDITOR
        private string DrawCustomLabelName()
        {
            return $"Prio {GetPriorityOrder()}";
        }
#endif

        protected bool IsPopupNull()
        {
            if (m_PopupInstance is UnityEngine.Object unityObj)
            {
                return unityObj == null;
            }
            return m_PopupInstance == null;
        }

        public virtual bool EvaluateCondition()
        {
            return true;
        }

        public virtual int GetPriorityOrder()
        {
            return m_Priority;
        }

        public virtual void Show()
        {
            popup.Show();
        }

        public virtual void Hide()
        {
            popup.Hide();
        }

        public virtual void ShowImmediately()
        {
            popup.ShowImmediately();
        }

        public virtual void HideImmediately()
        {
            popup.HideImmediately();
        }

        public virtual SubscriptionEvent GetOnStartShowEvent()
        {
            return popup.GetOnStartShowEvent();
        }

        public virtual SubscriptionEvent GetOnEndShowEvent()
        {
            return popup.GetOnEndShowEvent();
        }

        public virtual SubscriptionEvent GetOnStartHideEvent()
        {
            return popup.GetOnStartHideEvent();
        }

        public virtual SubscriptionEvent GetOnEndHideEvent()
        {
            return popup.GetOnEndHideEvent();
        }
    }
}