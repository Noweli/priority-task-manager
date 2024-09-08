using PriorityTaskManager.TaskManager.Models;

namespace PriorityTaskManager.TaskManager;

public sealed class PriorityPriorityTaskManager : IPriorityTaskManager
{
    private const int DefaultDelayMilliseconds = 500;
    private readonly SemaphoreSlim _semaphore;
    private readonly PriorityQueue<object, int> _taskQueue;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _processingTask;
    
    public PriorityPriorityTaskManager()
    {
        _semaphore = new SemaphoreSlim(1, 1);
        _taskQueue = new PriorityQueue<object, int>();
        _cancellationTokenSource = new CancellationTokenSource();
        _processingTask = ProcessTasks(_cancellationTokenSource.Token);
    }

    public Task<T> ScheduleTask<T>(Func<Task<T>> taskFunc, int priority)
    {
        var taskItem = new PriorityTask<T>(taskFunc, new TaskCompletionSource<T>());

        _semaphore.Wait();
        _taskQueue.Enqueue(taskItem, priority);
        _semaphore.Release();

        return taskItem.TaskCompletionSource.Task;
    }

    public async Task StopProcessing()
    {
        await _cancellationTokenSource.CancelAsync();
        await _processingTask.WaitAsync(_cancellationTokenSource.Token);
    }

    private async Task ProcessTasks(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_taskQueue.Count > 0)
            {
                dynamic taskItem = _taskQueue.Dequeue();

                try
                {
                    var result = await taskItem.TaskFunc();
                    taskItem.TaskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskItem.TaskCompletionSource.SetException(ex);
                }
            }
            else
            {
                await Task.Delay(DefaultDelayMilliseconds, cancellationToken);
            }
        }
    }
}