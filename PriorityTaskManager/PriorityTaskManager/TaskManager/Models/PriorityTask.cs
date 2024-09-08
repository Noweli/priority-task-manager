namespace PriorityTaskManager.TaskManager.Models;

public record PriorityTask<T>(Func<Task<T>> TaskFunc, TaskCompletionSource<T> TaskCompletionSource);