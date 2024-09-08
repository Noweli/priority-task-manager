using FluentAssertions;
using PriorityTaskManager.TaskManager;

namespace PriorityTaskManager.Tests.TaskManager.Tests;

[TestFixture]
public class PriorityTaskManagerTests
{
    private PriorityPriorityTaskManager _priorityTaskManager;

    [SetUp]
    public void SetUp()
    {
        _priorityTaskManager = new PriorityPriorityTaskManager();
    }

    [Test]
    public async Task ScheduleTask_ShouldReturnTaskResult()
    {
        // Arrange
        const int expected = 42;
        var task = Task.Run(async () =>
        {
            await Task.Delay(100);

            return expected;
        });

        // Act
        var result = await _priorityTaskManager.ScheduleTask(() => task, 1);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task ScheduleTwoTask_ShouldReturnInPriorityOrder()
    {
        // Arrange
        const int priority1TaskDelayMilliseconds = 200;
        const int priority2TaskDelayMilliseconds = 100;

        // Act
        var task1 = _priorityTaskManager.ScheduleTask(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(priority1TaskDelayMilliseconds));

            return DateTime.Now.Ticks;
        }, 1);

        var task2 = _priorityTaskManager.ScheduleTask(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(priority2TaskDelayMilliseconds));

            return DateTime.Now.Ticks;
        }, 2);

        var result1 = await task1;
        var result2 = await task2;

        // Assert
        result1.Should().BeLessThan(result2);
    }

    [Test]
    public async Task ScheduleThreeTasks_ShouldReturnInPriorityOrder()
    {
        // Arrange
        const int priority1TaskDelayMilliseconds = 300;
        const int priority2TaskDelayMilliseconds = 100;
        const int priority3TaskDelayMilliseconds = 50;

        // Act
        var task1 = _priorityTaskManager.ScheduleTask(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(priority1TaskDelayMilliseconds));

            return DateTime.Now.Ticks;
        }, 1);

        var task2 = _priorityTaskManager.ScheduleTask(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(priority2TaskDelayMilliseconds));

            return DateTime.Now.Ticks;
        }, 2);

        var task3 = _priorityTaskManager.ScheduleTask(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(priority3TaskDelayMilliseconds));

            return DateTime.Now.Ticks;
        }, 1);

        var result1 = await task1;
        var result2 = await task2;
        var result3 = await task3;

        // Assert
        result1.Should().BeLessThan(result2);
        result3.Should().BeLessThan(result2);
    }

    [Test]
    public void StopProcessing_ShouldStopProcessingTasks()
    {
        // Arrange
        var task = Task.Run(async () =>
        {
            await Task.Delay(100);

            return Task.CompletedTask;
        });
        
        // Act
        // ReSharper disable once ConvertToLocalFunction
        var taskToThrow = async () =>
        {
            var task1 = _priorityTaskManager.ScheduleTask(() => task, 1);
            var task2 = _priorityTaskManager.ScheduleTask(() => task, 1);
            await _priorityTaskManager.StopProcessing();
            await Task.WhenAll(task1, task2);
        };

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(() => taskToThrow());
    }
}