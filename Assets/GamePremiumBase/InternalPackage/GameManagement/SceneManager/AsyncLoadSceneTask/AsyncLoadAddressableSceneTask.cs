#if UNITY_ADDRESSABLES
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AsyncLoadAddressableSceneTask : IAsyncTask<SceneInstance>
{
    public AsyncLoadAddressableSceneTask(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        m_AsyncLoadSceneOperation = operationHandle;
        m_AsyncLoadSceneOperation.Completed += OnSceneLoadCompleted;

        void OnSceneLoadCompleted(AsyncOperationHandle<SceneInstance> _)
        {
            _onCompleted.Invoke();
        }
    }

    private event Action _onCompleted = delegate { };
    private AsyncOperationHandle<SceneInstance> m_AsyncLoadSceneOperation;

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
    public bool isCompleted => m_AsyncLoadSceneOperation.IsDone;
    public float percentageComplete => m_AsyncLoadSceneOperation.PercentComplete;
    public SceneInstance result => m_AsyncLoadSceneOperation.Result;
    public AsyncOperationHandle<SceneInstance> asyncOperation => m_AsyncLoadSceneOperation;
}
#endif