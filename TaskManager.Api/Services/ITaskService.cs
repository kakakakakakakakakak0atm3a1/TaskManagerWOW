using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;
namespace TaskManager.Api.Services;

public interface ITaskService
{
    Task<List<TaskItem>> GetTasksAsync(int userId, bool? isDone, DateTime? createdAfter, string? search, string? sortBy, int page, int pageSize);
    Task<TaskItem?> GetTaskByIdAsync(int userId, int taskId);
    Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto dto);
    Task<bool> DeleteTaskAsync(int userId, int taskId);
}