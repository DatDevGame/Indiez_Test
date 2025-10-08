using System;
using HCore.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Premium.PopupManagement
{
    [CreateAssetMenu(fileName = "PromotionalPopupSO", menuName = "Premium/Popup/PromotionalPopupSO")]
    public class PromotionalPopupSO : PopupSO
    {
        [SerializeField]
        protected bool m_RequireOnlineTime = true; // If true, cooldown time is only affected by time app is active (not paused)
        [SerializeField]
        protected float m_CooldownInSeconds = 0f; // How long before this popup can be shown again
        [SerializeField]
        protected int m_MaxFullPriceDisplayCount = -1; // -1 for unlimited, otherwise limit the number of times it shows
        [SerializeField]
        protected int m_MaxDiscountDisplayCount = -1; // -1 for unlimited, otherwise limit the number of times it shows

        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual float persistentCooldownInSeconds
        {
            get
            {
                return PlayerPrefs.GetFloat($"PopupPersistentCooldownInSeconds_{guid}", 0f);
            }
            set
            {
                PlayerPrefs.SetFloat($"PopupPersistentCooldownInSeconds_{guid}", value);
            }
        }
        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual float cooldownInSeconds { get; set; }
        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual int maxDisplayCount
        {
            get => isAbleToDiscount ? m_MaxDiscountDisplayCount : m_MaxFullPriceDisplayCount;
        }
        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual bool isAbleToDiscount
        {
            get => PlayerPrefs.GetInt($"PopupIsAbleToDiscount_{guid}", 0) == 1;
            set => PlayerPrefs.SetInt($"PopupIsAbleToDiscount_{guid}", value ? 1 : 0);
        }
        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual int discountDisplayCount
        {
            get => PlayerPrefs.GetInt($"PopupDiscountDisplayCount_{guid}", 0);
            set => PlayerPrefs.SetInt($"PopupDiscountDisplayCount_{guid}", value);
        }
        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual int fullPriceDisplayCount
        {
            get => PlayerPrefs.GetInt($"PopupFullPriceDisplayCount_{guid}", 0);
            set => PlayerPrefs.SetInt($"PopupFullPriceDisplayCount_{guid}", value);
        }
        [ShowInInspector, ReadOnly, TitleGroup("Debug"), PropertyOrder(100)]
        public virtual int displayCount
        {
            get => isAbleToDiscount ? discountDisplayCount : fullPriceDisplayCount;
            set
            {
                if (isAbleToDiscount)
                    discountDisplayCount = value;
                else
                    fullPriceDisplayCount = value;
            }
        }

        protected virtual void OnEnable()
        {
            cooldownInSeconds = persistentCooldownInSeconds;
            if (!m_RequireOnlineTime)
            {
                float offlineTimeInSeconds = (DateTime.Now - GlobalPopupConfigSO.Instance.lastAppPauseTime).Seconds;
                cooldownInSeconds -= offlineTimeInSeconds;
            }
            GameEventHandler.AddActionEvent(ApplicationLifecycleEventCode.OnApplicationPause, OnAppPause);
            GameEventHandler.AddActionEvent(ApplicationLifecycleEventCode.OnApplicationQuit, OnAppQuit);
        }

        protected virtual void OnDisable()
        {
            GameEventHandler.RemoveActionEvent(ApplicationLifecycleEventCode.OnApplicationPause, OnAppPause);
            GameEventHandler.RemoveActionEvent(ApplicationLifecycleEventCode.OnApplicationQuit, OnAppQuit);
        }

        protected virtual void OnAppPause(object[] parameters)
        {
            if (parameters[0] is bool pauseStatus && pauseStatus)
                persistentCooldownInSeconds = cooldownInSeconds;
        }

        protected virtual void OnAppQuit()
        {
            persistentCooldownInSeconds = cooldownInSeconds;
        }

        public virtual void Update()
        {
            cooldownInSeconds = Mathf.Max(cooldownInSeconds - Time.deltaTime, 0f);
        }

        public override bool EvaluateCondition()
        {
            if (cooldownInSeconds > 0f)
            {
                return false;
            }
            if (maxDisplayCount > -1 && displayCount >= maxDisplayCount)
            {
                return false;
            }
            return true;
        }

        [Button]
        public virtual void ResetData()
        {
            PlayerPrefs.DeleteKey($"PopupPersistentCooldownInSeconds_{guid}");
            PlayerPrefs.DeleteKey($"PopupIsAbleToDiscount_{guid}");
            PlayerPrefs.DeleteKey($"PopupDiscountDisplayCount_{guid}");
            PlayerPrefs.DeleteKey($"PopupFullPriceDisplayCount_{guid}");
        }

        public override void Show()
        {
            displayCount++;
            cooldownInSeconds = m_CooldownInSeconds;
            base.Show();
        }

        public override void ShowImmediately()
        {
            displayCount++;
            cooldownInSeconds = m_CooldownInSeconds;
            base.ShowImmediately();
        }
    }
}