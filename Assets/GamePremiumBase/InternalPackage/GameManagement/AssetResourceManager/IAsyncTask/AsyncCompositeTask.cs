using System;
using System.Linq;
using System.Collections.Generic;

public class AsyncCompositeTask : IAsyncTask
{
    public AsyncCompositeTask(IAsyncTask[] asyncTasks)
    {
        this.asyncTasks = asyncTasks;
        foreach (var asyncTask in asyncTasks)
        {
            asyncTask.onCompleted += OnCompleted;
        }

        void OnCompleted()
        {
            if (isCompleted)
                _onCompleted.Invoke();
        }
    }
    public AsyncCompositeTask(List<IAsyncTask> asyncTasks)
    {
        this.asyncTasks = asyncTasks.ToArray();
        foreach (var asyncTask in asyncTasks)
        {
            asyncTask.onCompleted += OnCompleted;
        }

        void OnCompleted()
        {
            if (isCompleted)
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

    public bool isCompleted => asyncTasks.All(task => task.isCompleted);
    public float percentageComplete => asyncTasks.Average(task => task.percentageComplete);
    public IAsyncTask[] asyncTasks { get; protected set; }
}