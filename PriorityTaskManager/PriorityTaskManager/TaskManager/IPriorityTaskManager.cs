namespace PriorityTaskManager.TaskManager;

public interface IPriorityTaskManager
{
    public Task<T> ScheduleTask<T>(Func<Task<T>> taskFunc, int priority);
    public Task StopProcessing();
}