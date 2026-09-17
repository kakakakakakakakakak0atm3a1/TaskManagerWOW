using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Services;
using TaskManager.Infrastructure.Data;
using TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Api.Mapping;
using Moq;

namespace TaskManager.Tests;



public class TaskServiceTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
    [Fact]
    public async Task GetTaskAsync_ReturnsOnlyTasksForGivenUser()
    {
        // Arrange
        var context = GetInMemoryContext();

        // Seed data
            context.Tasks.Add(new TaskItem { Id = 1, Title = "Task 1", UserId = 1 });
            context.Tasks.Add(new TaskItem { Id = 2, Title = "Task 2", UserId = 1 });
            context.Tasks.Add(new TaskItem { Id = 3, Title = "Task 3", UserId = 2 }); // Different user
        await context.SaveChangesAsync();
        var service = new TaskService(context);

        // Act
        var result = await service.GetTasksAsync(1, null, null, null, null, 1, 10);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(1, t.UserId));
    }
    [Fact]
    public async Task GetTaskByUserIdAsync_ReturnsNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryContext();

        context.Tasks.Add(new TaskItem { Id = 1, Title = "Task 1", UserId = 1 });
        context.Tasks.Add(new TaskItem { Id = 2, Title = "Task 2", UserId = 1 });
        await context.SaveChangesAsync();
        var service = new TaskService(context);

        // Act
        var result = await service.GetTaskByIdAsync(2, 1); // Non-existent task

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetAllTasks_RetunsNotFound_WhenTaskDoesNotExist()
    {
         // Arrange
         var context = GetInMemoryContext();
    var service = new TaskService(context);

    // Act
    var result = await service.GetTasksAsync(1, null, null, null, null, 1, 10);

    // Assert
    Assert.Empty(result);
    }

    [Fact]

    public async Task DeleteTaskAsync_ReturnsFalse_AndDoesNotDelete_WhereTaskBelongsToAnotherUser()
    {
        // Arrange
         var context = GetInMemoryContext();
          // Seed data
            context.Tasks.Add(new TaskItem { Id = 1, Title = "Task 1", UserId = 1 });
            context.Tasks.Add(new TaskItem { Id = 2, Title = "Task 2", UserId = 1 });
            context.Tasks.Add(new TaskItem { Id = 3, Title = "Task 3", UserId = 1 });
            await context.SaveChangesAsync(); 

    var service = new TaskService(context);

    // Act
    var result = await service.DeleteTaskAsync(2,1);

    // Assert
    Assert.False(result);
    var taskStillExists = await context.Tasks.AnyAsync(t => t.Id == 1);
    Assert.True(taskStillExists);
    }
    
}