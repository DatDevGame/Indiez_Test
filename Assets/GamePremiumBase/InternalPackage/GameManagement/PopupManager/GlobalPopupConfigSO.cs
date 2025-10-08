using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using HCore.Events;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Premium.PopupManagement
{
    /// <summary>
    /// Defines a rate limit config and associated popups that follow it
    /// Used to override default popup display limits on a per-popup basis
    /// </summary>
    [Serializable]
    public class PromotionalPopupLimitConfig
    {
        [SerializeField]
        private int m_PromotionalPopupRateLimit = -1; // -1 for unlimited, otherwise limit the number of popups that can be shown at a time
        [SerializeField]
        private PopupSO[] m_ConfiguredPopupSOs = new PopupSO[0]; // Popups that use this rate limit override

        public int promotionalPopupRateLimit => m_PromotionalPopupRateLimit;
        public PopupSO[] configuredPopupSOs => m_ConfiguredPopupSOs;
    }
    [GlobalConfig("Assets/Resources")]
    [WindowMenuItem("GlobalConfig", "Popup", "Assets/Resources")]
    public class GlobalPopupConfigSO : GlobalConfig<GlobalPopupConfigSO>
    {
        [SerializeField]
        private bool m_AutoRunOnLoadSceneCompleted = true; // Auto-show popups after load main menu scene completed
        [SerializeField, ShowIf("m_AutoRunOnLoadSceneCompleted")]
        private SceneName m_MainMenuSceneName; // Name of the main scene to check after load completion
        [SerializeField]
        private int m_DefaultPromotionalPopupRateLimit = -1; // Default popup limit (-1 for unlimited, otherwise limit the number of popups that can be shown at a time)
        [SerializeField, ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "DrawCustomLabelName")]
        private PopupSO[] m_PredefinedPopupSOs = new PopupSO[0]; // List of all predefined popups
        [SerializeField]
        private PromotionalPopupLimitConfig[] m_OverridePromotionalPopupLimitConfigs = new PromotionalPopupLimitConfig[0]; // Optional promotional popup display limit configs

        public bool autoRunOnLoadSceneCompleted => m_AutoRunOnLoadSceneCompleted;
        public string mainMenuSceneName => m_MainMenuSceneName.ToString();
        public int defaultPromotionalPopupRateLimit => m_DefaultPromotionalPopupRateLimit;
        public long lastAppPauseTimeTicks
        {
            get
            {
                return long.Parse(PlayerPrefs.GetString("LastAppPauseTimeTicks", 0.ToString(CultureInfo.InvariantCulture)));
            }
            set
            {
                PlayerPrefs.SetString("LastAppPauseTimeTicks", value.ToString(CultureInfo.InvariantCulture));
            }
        }
        public DateTime lastAppPauseTime
        {
            get
            {
                return new DateTime(lastAppPauseTimeTicks);
            }
            set
            {
                lastAppPauseTimeTicks = value.Ticks;
            }
        }
        public PopupSO[] predefinedPopupSOs => m_PredefinedPopupSOs;
        public PromotionalPopupLimitConfig[] overridePromotionalPopupLimitConfigs => m_OverridePromotionalPopupLimitConfigs;

        private void OnEnable()
        {
            SortPredefinedPopups();
            GameEventHandler.AddActionEvent(ApplicationLifecycleEventCode.OnApplicationPause, OnAppPause);
            GameEventHandler.AddActionEvent(ApplicationLifecycleEventCode.OnApplicationQuit, OnAppQuit);
        }

        private void OnDisable()
        {
            GameEventHandler.RemoveActionEvent(ApplicationLifecycleEventCode.OnApplicationPause, OnAppPause);
            GameEventHandler.RemoveActionEvent(ApplicationLifecycleEventCode.OnApplicationQuit, OnAppQuit);
        }

        private void OnValidate()
        {
            SortPredefinedPopups();
        }

        private void OnAppPause(object[] parameters)
        {
            if (parameters[0] is bool pauseStatus && pauseStatus)
            {
                lastAppPauseTime = DateTime.Now;
            }
        }

        private void OnAppQuit()
        {
            lastAppPauseTime = DateTime.Now;
        }

        [OnInspectorInit, Conditional("UNITY_EDITOR")]
        private void OnInspectorInit()
        {
            SortPredefinedPopups();
        }

        private void SortPredefinedPopups()
        {
            m_PredefinedPopupSOs.Sort((a, b) => b.GetPriorityOrder().CompareTo(a.GetPriorityOrder()));
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public int GetOverridePromotionalPopupRateLimit(PopupSO popupSO)
        {
            foreach (var popupLimitConfig in m_OverridePromotionalPopupLimitConfigs)
            {
                if (popupLimitConfig.configuredPopupSOs.Contains(popupSO))
                    return popupLimitConfig.promotionalPopupRateLimit;
            }
            return -1;
        }
    }
}