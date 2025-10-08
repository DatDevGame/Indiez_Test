#if UNITY_ADDRESSABLES
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AsyncUnloadAddressableSceneTask : IAsyncTask<SceneInstance>
{
    public AsyncUnloadAddressableSceneTask(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        m_AsyncUnloadSceneOperation = operationHandle;
        m_AsyncUnloadSceneOperation.Completed += OnSceneUnloadCompleted;

        void OnSceneUnloadCompleted(AsyncOperationHandle<SceneInstance> _)
        {
            _onCompleted.Invoke();
        }
    }

    private event Action _onCompleted = delegate { };
    private AsyncOperationHandle<SceneInstance> m_AsyncUnloadSceneOperation;

    public event Action onCompleted
    {
        add
        {
            if (isCompleted)
                value?.Invoke();
            _onCompleted += value;
        }
        remove
        {
            _onCompleted -= value;
        }
    }
    public bool isCompleted => m_AsyncUnloadSceneOperation.IsDone;
    public float percentageComplete => m_AsyncUnloadSceneOperation.PercentComplete;
    public SceneInstance result => m_AsyncUnloadSceneOperation.Result;
    public AsyncOperationHandle<SceneInstance> asyncOperation => m_AsyncUnloadSceneOperation;
}
#endif