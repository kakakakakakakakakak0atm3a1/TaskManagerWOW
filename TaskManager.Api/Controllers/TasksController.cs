using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

 namespace TaskManager.Api.Controllers;

 [Authorize]
 [ApiController]
 [Route("api/[controller]")]
 public class TasksController : ControllerBase
 {
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public TasksController(AppDbContext context, IMapper mapper)
    {
        _context = context;
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
     
        IQueryable<TaskItem> query = _context.Tasks.Where(t => t.UserId == userId);
        
        if (isDone.HasValue)
        {
            query = query.Where(t => t.IsDone == isDone.Value);
            
        }
        if (createdAfter.HasValue)
        {
            query = query.Where(t => t.CreatedAt > createdAfter.Value);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
         query = query.Where(t => t.Title.Contains(search));
        }
        query = sortBy switch
        {
            "title" => query.OrderBy(t => t.Title),
            "createdAt" => query.OrderByDescending(t => t.CreatedAt),
            _ => query.OrderBy(t => t.Id)
        };
        var tasks = await query
        
        .Skip((page-1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
        var tasksDto =  _mapper.Map<List<TaskResponseDto>>(tasks);
        return Ok(tasksDto);
    }
[HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
    {
       
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId ==  userId);
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
        var task = _mapper.Map<TaskItem>(createTaskDto);
        task.IsDone = false;
        task.CreatedAt = DateTime.UtcNow;
        task.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); 
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var taskResponseDto = _mapper.Map<TaskResponseDto>(task);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, taskResponseDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
         var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
         
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId ==  userId);

        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

 }