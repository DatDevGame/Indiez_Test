using System;
using UnityEngine;

public class AsyncLoadSceneTask : IAsyncTask<AsyncOperation>
{
    public AsyncLoadSceneTask (AsyncOperation asyncOperation)
    {
        if (asyncOperation == null)
            return;
        this.asyncOperation = asyncOperation;
        this.asyncOperation.completed += OnSceneLoadCompleted;

        void OnSceneLoadCompleted(AsyncOperation asyncOperation)
        {
            _onCompleted.Invoke();
        }
    }

    private event Action _onCompleted = delegate { };
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

    public bool isCompleted => asyncOperation.isDone;
    public float percentageComplete => asyncOperation.progress;
    public AsyncOperation result => asyncOperation;
    public AsyncOperation asyncOperation { get; protected set; }
}