using System;
using System.Collections.Generic;
using HightLightDebug;
using HCore.Events;
using UnityEngine;

namespace Premium.PopupManagement
{
    public class ShowPopupRequest
    {
        public IPopup popup { get; set; }
        public bool isShowImmediately { get; set; } = false;
    }
    public class PopupManager : Singleton<PopupManager>
    {
        private const string k_DebugTag = nameof(PopupManager);
        private static readonly List<PromotionalPopupSO> s_PromotionalPopupSOs = new List<PromotionalPopupSO>();
        private static readonly PriorityQueue<ShowPopupRequest, int> s_PopupPriorityQueue = new PriorityQueue<ShowPopupRequest, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        private void Start()
        {
            foreach (var predefinedPopupSO in GetConfigSO().predefinedPopupSOs)
            {
                if (predefinedPopupSO is PromotionalPopupSO promotionalPopupSO)
                    s_PromotionalPopupSOs.Add(promotionalPopupSO);
            }
            GameEventHandler.AddActionEvent(SceneManagementEventCode.OnLoadSceneCompleted, OnLoadSceneCompleted);
        }

        private void OnDestroy()
        {
            GameEventHandler.RemoveActionEvent(SceneManagementEventCode.OnLoadSceneCompleted, OnLoadSceneCompleted);
        }

        private void Update()
        {
            foreach (var predefinedPopupSO in s_PromotionalPopupSOs)
            {
                predefinedPopupSO.Update();
            }
        }

        private static GlobalPopupConfigSO GetConfigSO()
        {
            return GlobalPopupConfigSO.Instance;
        }

        private static void OnLoadSceneCompleted(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                return;
            GlobalPopupConfigSO configSO = GetConfigSO();
            string destinationSceneName = parameters[0] as string;
            if (configSO.autoRunOnLoadSceneCompleted && destinationSceneName == configSO.mainMenuSceneName)
            {
                ShowAvailablePopups();
            }
        }

        private static void ShowPendingPopups(bool isInitialCall = true)
        {
            if (s_PopupPriorityQueue.Count <= 0)
                return;
            ShowPopupRequest request = s_PopupPriorityQueue.Dequeue();
            if (!request.isShowImmediately)
                request.popup.Show();
            else
                request.popup.ShowImmediately();
            request.popup.GetOnEndHideEvent().Subscribe(OnPopupClosed);
            GameEventHandler.Invoke(PopupManagementEventCode.OnPopupShowed, request);
            if (isInitialCall)
                GameEventHandler.Invoke(PopupManagementEventCode.OnFirstPopupShowed, request);
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Show popup - {request.popup}");

            void OnPopupClosed()
            {
                request.popup.GetOnEndHideEvent().Unsubscribe(OnPopupClosed);
                GameEventHandler.Invoke(PopupManagementEventCode.OnPopupClosed, request);
                if (s_PopupPriorityQueue.Count <= 0)
                    GameEventHandler.Invoke(PopupManagementEventCode.OnLastPopupClosed, request);
                ShowPendingPopups(false);
            }
        }

        public static PriorityQueue<ShowPopupRequest, int> GetPopupPriorityQueue()
        {
            return s_PopupPriorityQueue;
        }

        public static ShowPopupRequest CreateShowRequest(IPopup popup, bool isShowImmediately)
        {
            return new ShowPopupRequest()
            {
                popup = popup,
                isShowImmediately = isShowImmediately
            };
        }

        public static void EnqueuePopup(IPopup popup, bool isShowImmediately)
        {
            EnqueueShowPopupRequest(CreateShowRequest(popup, isShowImmediately));
        }

        public static void EnqueueShowPopupRequest(ShowPopupRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.popup == null)
                throw new ArgumentNullException(nameof(request.popup));
            s_PopupPriorityQueue.Enqueue(request, request.popup.GetPriorityOrder());
            if (Instance.m_Verbose)
                Debug.LogError($"Enqueue show popup request - {request.popup}");
        }

        public static bool Contains(IPopup popup)
        {
            foreach (var item in s_PopupPriorityQueue.UnorderedItems)
            {
                if (item.Element.popup == popup)
                {
                    return true;
                }
            }
            return false;
        }

        public static void Clear()
        {
            s_PopupPriorityQueue.Clear();
        }

        public static void ShowPendingPopups()
        {
            ShowPendingPopups(true);
        }

        public static void EnqueueAvailablePopups()
        {
            EnqueueAvailablePopups(GetConfigSO().defaultPromotionalPopupRateLimit);
        }

        public static void EnqueueAvailablePopups(int promotionalPopupRateLimit)
        {
            GlobalPopupConfigSO configSO = GetConfigSO();
            if (configSO.predefinedPopupSOs.Length <= 0)
                return;

            int limitCount = 0;
            foreach (var predefinedPopupSO in configSO.predefinedPopupSOs)
            {
                if (!predefinedPopupSO.EvaluateCondition())
                {
                    continue;
                }
                if (predefinedPopupSO is PromotionalPopupSO)
                {
                    if (promotionalPopupRateLimit > -1 && limitCount >= promotionalPopupRateLimit)
                    {
                        continue;
                    }
                    limitCount++;
                }
                int overridePromotionalPopupRateLimit = configSO.GetOverridePromotionalPopupRateLimit(predefinedPopupSO);
                if (overridePromotionalPopupRateLimit > -1)
                {
                    if (promotionalPopupRateLimit > -1)
                    {
                        promotionalPopupRateLimit = Mathf.Min(promotionalPopupRateLimit, overridePromotionalPopupRateLimit);
                    }
                    else
                    {
                        promotionalPopupRateLimit = overridePromotionalPopupRateLimit;
                    }
                }
                EnqueuePopup(predefinedPopupSO, predefinedPopupSO.isShowImmediately);
            }
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Total requests: {s_PopupPriorityQueue.Count}");
        }

        public static void ShowAvailablePopups()
        {
            ShowAvailablePopups(GetConfigSO().defaultPromotionalPopupRateLimit);
        }

        public static void ShowAvailablePopups(int promotionalPopupRateLimit)
        {
            EnqueueAvailablePopups(promotionalPopupRateLimit);
            ShowPendingPopups();
        }
    }
}