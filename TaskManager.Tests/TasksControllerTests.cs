using TaskManager.Infrastructure.Data;
using TaskManager.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Api.Mapping;
using TaskManager.Api.DTOs;
using TaskManager.Api.Services;
using Moq;
using TaskManager.Domain.Entities;
namespace TaskManager.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly IMapper _mapper;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

        _mapper = config.CreateMapper();
    }
    private TasksController CreateController(int userId)
    {
        var controller = new TasksController(_taskServiceMock.Object, _mapper);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        return controller;
    }

    [Fact]
    public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var controller = CreateController(1);
        // Setup mock
        _taskServiceMock.Setup(s => s.GetTaskByIdAsync(1, 1)).ReturnsAsync((TaskItem?)null);
        // Act
        var result = await controller.GetTask(1);
        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
     
    [Fact]
    public async Task GetAll_ReturnsTasks_ForAuthenticatedUser()
    {
        // Arrange
        var controller = CreateController(1);
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Task 1", UserId = 1 },
            new TaskItem { Id = 2, Title = "Task 2", UserId = 1 },
           // Different user
        };

        // Setup mock
       _taskServiceMock.Setup(s => s.GetTasksAsync(1, null, null, null, null, 1, 10))
            .ReturnsAsync(tasks);
        

        // Act
        var result = await controller.GetAll(null, null, null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var resultTasks = Assert.IsAssignableFrom<IEnumerable<TaskResponseDto>>(okResult.Value);
        Assert.Equal(2, resultTasks.Count()); 
    }
   
    [Fact]

    public async Task GetTask_ReturnsTask_ForAuthenticatedUser()
    {
        // Arrange
        
        var controller = CreateController(1);
      

        // Setup mock
        var task = new TaskItem { Id = 1, Title = "Task 1", UserId = 1 };
        _taskServiceMock.Setup(s => s.GetTaskByIdAsync(1, 1)).ReturnsAsync(task);

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
       var controller = CreateController(1);
       // Setup mock
        _taskServiceMock.Setup(s => s.DeleteTaskAsync(1, 1)).ReturnsAsync(false);
        // Act
        var result = await controller.DeleteTask(1);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]

    public async Task DeleteTask_ReturnsNoContent_WhenTaskExists()
    {
        // Arrange
        var controller = CreateController(1);
        // Setup mock
        _taskServiceMock.Setup(s => s.DeleteTaskAsync(1, 1)).ReturnsAsync(true);
        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

}

