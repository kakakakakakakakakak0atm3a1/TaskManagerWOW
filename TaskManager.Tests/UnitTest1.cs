using TaskManager.Infrastructure.Data;
using TaskManager.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Api.Mapping;
using TaskManager.Api.DTOs;

namespace TaskManager.Tests;

public class TasksControllerTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        return context;
    }
    private void SetUserContext(TasksController controller, int userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new TasksController(context, mapper: GetMapper());
        SetUserContext(controller, 1);
        // Act
        var result = await controller.GetTask(1);
        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
       private IMapper GetMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }
    [Fact]
    public async Task GetAll_ReturnsTasks_ForAuthenticatedUser()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new TasksController(context, mapper: GetMapper());
        SetUserContext(controller, 1);

        // Seed data
        context.Tasks.Add(new TaskManager.Domain.Entities.TaskItem { Id = 1, Title = "Task 1", UserId = 1 });
        context.Tasks.Add(new TaskManager.Domain.Entities.TaskItem { Id = 2, Title = "Task 2", UserId = 2 });
        context.Tasks.Add(new TaskManager.Domain.Entities.TaskItem { Id = 3, Title = "Task 3", UserId = 1 });
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetAll(null, null, null, null);

        // Assert
       var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var tasks = Assert.IsAssignableFrom<IEnumerable<TaskResponseDto>>(okResult.Value);
        Assert.Equal(2, tasks.Count()); // Only tasks for user with ID 1 should be returned
    }
   
    [Fact]

    public async Task GetTask_ReturnsTask_ForAuthenticatedUser()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new TasksController(context, mapper: GetMapper());
        SetUserContext(controller, 1);

        // Seed data
        var task = new TaskManager.Domain.Entities.TaskItem { Id = 1, Title = "Task 1", UserId = 1 };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetTask(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var taskDto = Assert.IsType<TaskResponseDto>(okResult.Value);
        Assert.Equal(task.Id, taskDto.Id);
        Assert.Equal(task.Title, taskDto.Title);
    }
    [Fact]
    public async Task DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new TasksController(context, mapper: GetMapper());
        SetUserContext(controller, 1);
        // Act
        var result = await controller.DeleteTask(1);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]

    public async Task DeleteTask_ReturnsNoContent_WhenTaskExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new TasksController(context, mapper: GetMapper());
        SetUserContext(controller, 1);

        // Seed data
        var task = new TaskManager.Domain.Entities.TaskItem { Id = 1, Title = "Task 1", UserId = 1 };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

}

