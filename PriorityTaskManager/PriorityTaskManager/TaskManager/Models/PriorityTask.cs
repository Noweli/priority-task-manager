namespace PriorityTaskManager.TaskManager.Models;

public record PriorityTask<T>(Func<Task<T>> TaskFunc, int Priority, TaskCompletionSource<T> TaskCompletionSource);