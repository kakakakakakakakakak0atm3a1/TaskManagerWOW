using TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetTasksAsync(int userId, bool? isDone, DateTime? createdAfter, string? search, string? sortBy, int page, int pageSize)
    {
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

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
    public async Task<TaskItem?> GetTaskByIdAsync(int userId, int taskId)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
       
    }
    public async Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            IsDone = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return task;
    }
    public async Task<bool> DeleteTaskAsync(int userId, int taskId)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}