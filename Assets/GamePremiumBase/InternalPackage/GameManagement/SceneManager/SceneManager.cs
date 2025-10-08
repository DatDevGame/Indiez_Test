using System;
using System.Linq;
using System.Collections.Generic;
using HCore.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using HightLightDebug;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace Premium.GameManagement
{
    public class SceneManager : Singleton<SceneManager>
    {
        public enum Status
        {
            Succeeded,
            Failed
        }
        public class LoadSceneRequest
        {
            [ShowInInspector]
            public bool isPushToStack { get; set; }
            [ShowInInspector]
            public bool isSetActiveScene { get; set; }
            [ShowInInspector]
            public LoadSceneMode mode { get; set; }
            [ShowInInspector]
            public string originSceneName { get; set; }
            [ShowInInspector]
            public string destinationSceneName { get; set; }
        }
        public class LoadSceneResponse
        {
            public Status status { get; set; }
            public string errorMessage { get; set; }
        }

        private const string k_DebugTag = nameof(SceneManager);
        private static bool s_IsLoadingScene;
        [ShowInInspector, ReadOnly, Title("DEBUG")]
        private static Stack<LoadSceneRequest> s_LoadSceneRequestStack = new Stack<LoadSceneRequest>();
        private static Dictionary<LoadSceneRequest, IAsyncTask> s_AsyncLoadSceneTaskDictionary = new Dictionary<LoadSceneRequest, IAsyncTask>();

        private static void PreLoadScene(LoadSceneRequest request)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Prepare to load scene {request.originSceneName} -> {request.destinationSceneName}");
            s_IsLoadingScene = true;
            if (request.isPushToStack)
            {
                s_LoadSceneRequestStack.Push(request);
            }
            GameEventHandler.Invoke(SceneManagementEventCode.OnStartLoadScene, request.destinationSceneName, request.originSceneName, request);
        }

        private static void PostLoadScene(LoadSceneRequest request, IAsyncTask task)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Load scene {request.originSceneName} -> {request.destinationSceneName} process started");
            if (task != null)
            {
                s_AsyncLoadSceneTaskDictionary.Add(request, task);
            }
            GameEventHandler.Invoke(SceneManagementEventCode.OnLoadSceneStarted, request.destinationSceneName, request.originSceneName, request, task);
        }

        private static void PreUnloadScene(string sceneName)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Prepare to unload scene {sceneName}");
            GameEventHandler.Invoke(SceneManagementEventCode.OnStartUnloadScene, sceneName);
        }

        private static void PostUnloadScene(string sceneName, IAsyncTask task)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Unload scene {sceneName} process started");
            GameEventHandler.Invoke(SceneManagementEventCode.OnUnloadSceneStarted, sceneName, task);
        }

        private static void NotifyEventLoadSceneCompleted(LoadSceneRequest request, Action<LoadSceneResponse> callback)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Load scene {request.destinationSceneName} completed");
            s_IsLoadingScene = false;
            callback?.Invoke(new LoadSceneResponse()
            {
                status = Status.Succeeded,
                errorMessage = string.Empty,
            });
            GameEventHandler.Invoke(SceneManagementEventCode.OnLoadSceneCompleted, request.destinationSceneName, request.originSceneName, request);
        }

        private static void NotifyEventUnloadSceneCompleted(string sceneName, Action callback)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Unload scene {sceneName} completed");
            callback?.Invoke();
            GameEventHandler.Invoke(SceneManagementEventCode.OnUnloadSceneCompleted, sceneName);
        }

        private static void SetActiveSceneInternal(string sceneName)
        {
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Set active scene {sceneName}");
            Scene targetScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (targetScene.isLoaded)
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(targetScene);
        }

        private static void OnLoadSceneCompleted(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (request.mode == LoadSceneMode.Additive && request.isSetActiveScene)
                SetActiveSceneInternal(request.destinationSceneName);
            NotifyEventLoadSceneCompleted(request, callback);
        }

        private static void LoadSceneInternal(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(request.destinationSceneName, request.mode);
            Instance.StartCoroutine(CommonCoroutine.Wait(null, OnLoadCompleted));

            void OnLoadCompleted()
            {
                OnLoadSceneCompleted(request, callback);
            }
        }

        private static AsyncLoadSceneTask LoadSceneAsyncInternal(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(request.destinationSceneName, request.mode);
            asyncOperation.allowSceneActivation = true;
            asyncOperation.priority = 100;
            asyncOperation.completed += OnLoadCompleted;

            void OnLoadCompleted(AsyncOperation _)
            {
                OnLoadSceneCompleted(request, callback);
            }

            return new AsyncLoadSceneTask(asyncOperation);
        }

#if UNITY_ADDRESSABLES
        private static AsyncLoadAddressableSceneTask LoadAddressableSceneAsyncInternal(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            AsyncOperationHandle<SceneInstance> asyncOperation = Addressables.LoadSceneAsync(AddressablesUtility.FindAssetKeyBySceneName(request.destinationSceneName), request.mode, true);
            asyncOperation.Completed += OnLoadCompleted;

            void OnLoadCompleted(AsyncOperationHandle<SceneInstance> _)
            {
                OnLoadSceneCompleted(request, callback);
            }

            return new AsyncLoadAddressableSceneTask(asyncOperation);
        }
#endif

        private static AsyncUnloadSceneTask UnloadSceneAsyncInternal(string sceneName, UnloadSceneOptions options, Action callback = null)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName, options);
            asyncOperation.completed += OnUnloadSceneCompleted;

            void OnUnloadSceneCompleted(AsyncOperation _)
            {
                Resources.UnloadUnusedAssets();
                NotifyEventUnloadSceneCompleted(sceneName, callback);
            }

            return new AsyncUnloadSceneTask(asyncOperation);
        }

#if UNITY_ADDRESSABLES
        private static AsyncUnloadAddressableSceneTask UnloadAddressableSceneAsyncInternal(string sceneName, UnloadSceneOptions options, Action callback = null)
        {
            IAsyncTask asyncLoadSceneTask = s_AsyncLoadSceneTaskDictionary.Values.FirstOrDefault(loadSceneTask => loadSceneTask is AsyncLoadAddressableSceneTask loadAddressableSceneTask && loadAddressableSceneTask.isCompleted && loadAddressableSceneTask.result.Scene.name == sceneName);
            AsyncOperationHandle<SceneInstance> asyncOperation = Addressables.UnloadSceneAsync((asyncLoadSceneTask as AsyncUnloadAddressableSceneTask).asyncOperation, options);
            asyncOperation.Completed += OnUnloadSceneCompleted;

            void OnUnloadSceneCompleted(AsyncOperationHandle<SceneInstance> _)
            {
                NotifyEventUnloadSceneCompleted(sceneName, callback);
            }

            return new AsyncUnloadAddressableSceneTask(asyncOperation);
        }
#endif

        private static bool ValidateRequest(LoadSceneRequest request, Action<LoadSceneResponse> callback)
        {
            if (s_IsLoadingScene)
            {
                if (Instance.m_Verbose)
                    DebugPro.AquaBold($"Failed to load scene because another scene is being loaded in-progress");
                callback?.Invoke(new LoadSceneResponse()
                {
                    status = Status.Failed,
                    errorMessage = "Another scene is being loaded in-progress"
                });
                return false;
            }
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(request.destinationSceneName).isLoaded)
            {
                if (Instance.m_Verbose)
                    DebugPro.AquaBold($"Failed to load scene because this scene ({request.destinationSceneName}) has already been loaded");
                callback?.Invoke(new LoadSceneResponse()
                {
                    status = Status.Failed,
                    errorMessage = $"This scene ({request.destinationSceneName}) has already been loaded"
                });
                return false;
            }
            return true;
        }

        public static LoadSceneRequest CreateLoadSceneRequest(string destinationSceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true)
        {
            return new LoadSceneRequest()
            {
                mode = mode,
                isPushToStack = isPushToStack,
                isSetActiveScene = isSetActiveScene,
                destinationSceneName = destinationSceneName,
                originSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            };
        }

        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            LoadScene(CreateLoadSceneRequest(sceneName, mode, isPushToStack, isSetActiveScene), callback);
        }

        public static void LoadScene(SceneName sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            LoadScene(sceneName.ToString(), mode, isPushToStack, isSetActiveScene, callback);
        }

        public static void LoadScene(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (!ValidateRequest(request, callback))
                return;
            PreLoadScene(request);
            LoadSceneInternal(request, callback);
            PostLoadScene(request, null);
        }

        public static AsyncLoadSceneTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            return LoadSceneAsync(CreateLoadSceneRequest(sceneName, mode, isPushToStack, isSetActiveScene), callback);
        }

        public static AsyncLoadSceneTask LoadSceneAsync(SceneName sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            return LoadSceneAsync(sceneName.ToString(), mode, isPushToStack, isSetActiveScene, callback);
        }

        public static AsyncLoadSceneTask LoadSceneAsync(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (!ValidateRequest(request, callback))
                return null;
            PreLoadScene(request);
            AsyncLoadSceneTask asyncLoadSceneTask = LoadSceneAsyncInternal(request, callback);
            PostLoadScene(request, asyncLoadSceneTask);
            return asyncLoadSceneTask;
        }

#if UNITY_ADDRESSABLES
        public static AsyncLoadAddressableSceneTask LoadAddressableSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            return LoadAddressableSceneAsync(CreateLoadSceneRequest(sceneName, mode, isPushToStack, isSetActiveScene), callback);
        }

        public static AsyncLoadAddressableSceneTask LoadAddressableSceneAsync(SceneName sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            return LoadAddressableSceneAsync(sceneName.ToString(), mode, isPushToStack, isSetActiveScene, callback);
        }

        public static AsyncLoadAddressableSceneTask LoadAddressableSceneAsync(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (!ValidateRequest(request, callback))
                return null;
            PreLoadScene(request);
            AsyncLoadAddressableSceneTask asyncLoadSceneTask = LoadAddressableSceneAsyncInternal(request, callback);
            PostLoadScene(request, asyncLoadSceneTask);
            return asyncLoadSceneTask;
        }
#endif

        public static AsyncUnloadSceneTask UnloadSceneAsync(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action callback = null)
        {
            PreUnloadScene(sceneName);
            AsyncUnloadSceneTask asyncUnloadSceneTask = UnloadSceneAsyncInternal(sceneName, options, callback);
            PostUnloadScene(sceneName, asyncUnloadSceneTask);
            return asyncUnloadSceneTask;
        }

        public static AsyncUnloadSceneTask UnloadSceneAsync(SceneName sceneName, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action callback = null)
        {
            return UnloadSceneAsync(sceneName.ToString(), options, callback);
        }

        public static AsyncUnloadSceneTask UnloadSceneAsync(LoadSceneRequest request, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action callback = null)
        {
            return UnloadSceneAsync(request.destinationSceneName);
        }

#if UNITY_ADDRESSABLES
        public static AsyncUnloadAddressableSceneTask UnloadAddressableSceneAsync(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action callback = null)
        {
            PreUnloadScene(sceneName);
            AsyncUnloadAddressableSceneTask asyncUnloadSceneTask = UnloadAddressableSceneAsyncInternal(sceneName, options, callback);
            PostUnloadScene(sceneName, asyncUnloadSceneTask);
            return asyncUnloadSceneTask;
        }

        public static AsyncUnloadAddressableSceneTask UnloadAddressableSceneAsync(SceneName sceneName, UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action callback = null)
        {
            return UnloadAddressableSceneAsync(sceneName.ToString(), options, callback);
        }
#endif

        public static void BackToPreviousScene(Action callback = null)
        {
            if (s_LoadSceneRequestStack.Count <= 0)
                return;
            LoadSceneRequest request = s_LoadSceneRequestStack.Pop();
            if (request == null)
                return;
            if (request.mode == LoadSceneMode.Single || !GetSceneByName(request.originSceneName).isLoaded)
            {
                LoadScene(request.originSceneName, isPushToStack: false, callback: _ => callback?.Invoke());
            }
            else
            {
                UnloadSceneAsync(request, callback: callback);
            }
            s_AsyncLoadSceneTaskDictionary.Remove(request);
        }

        public static IAsyncTask BackToPreviousSceneAsync(Action callback = null)
        {
            if (s_LoadSceneRequestStack.Count <= 0)
                return null;
            LoadSceneRequest request = s_LoadSceneRequestStack.Pop();
            if (request == null)
                return null;
            IAsyncTask asyncTask;
            if (request.mode == LoadSceneMode.Single || !GetSceneByName(request.originSceneName).isLoaded)
            {
                asyncTask = LoadSceneAsync(request.originSceneName, isPushToStack: false, callback: _ => callback?.Invoke());
            }
            else
            {
                asyncTask = UnloadSceneAsync(request, callback: callback);
            }
            s_AsyncLoadSceneTaskDictionary.Remove(request);
            return asyncTask;
        }

#if UNITY_ADDRESSABLES
        public static IAsyncTask BackToPreviousAddressableSceneAsync(Action callback = null)
        {
            if (s_LoadSceneRequestStack.Count <= 0)
                return null;
            LoadSceneRequest request = s_LoadSceneRequestStack.Pop();
            if (request == null)
                return null;
            IAsyncTask asyncTask;
            if (request.mode == LoadSceneMode.Single || !GetSceneByName(request.originSceneName).isLoaded)
            {
                asyncTask = LoadAddressableSceneAsync(request.originSceneName, isPushToStack: false, callback: _ => callback?.Invoke());
            }
            else
            {
                asyncTask = UnloadAddressableSceneAsync(request.destinationSceneName, callback: callback);
            }
            s_AsyncLoadSceneTaskDictionary.Remove(request);
            return asyncTask;
        }
#endif

        public static Scene GetSceneByName(string sceneName)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        }

        public static Scene GetSceneByBuildIndex(int buildIndex)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(buildIndex);
        }

        public static Scene GetSceneAt(int index)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneAt(index);
        }

        public static Scene GetActiveScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }
    }
}