using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManager.Api.Services;


 namespace TaskManager.Api.Controllers;

 [Authorize]
 [ApiController]
 [Route("api/[controller]")]
 public class TasksController : ControllerBase
 {
    private readonly ITaskService _taskService;
    private readonly IMapper _mapper;
    
    public TasksController(ITaskService taskService, IMapper mapper)
    {
       
        _taskService = taskService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll(
        [FromQuery] bool? isDone,
        [FromQuery] DateTime? createdAfter,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
     
        
        var tasks = await _taskService.GetTasksAsync(userId, isDone, createdAfter, search, sortBy, page, pageSize);
        var tasksDto =  _mapper.Map<List<TaskResponseDto>>(tasks);
        return Ok(tasksDto);
    }
[HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
    {
       
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var task = await _taskService.GetTaskByIdAsync(userId, id);
        if (task == null)
        {
            return NotFound();
        }
        var taskDto = _mapper.Map<TaskResponseDto>(task);
        return Ok(taskDto);
    }
    [HttpPost]
     public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto createTaskDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var task = await _taskService.CreateTaskAsync(userId, createTaskDto);
        var taskResponseDto = _mapper.Map<TaskResponseDto>(task);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, taskResponseDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
         var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
         var deleted = await _taskService.DeleteTaskAsync(userId, id);
       
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

 }