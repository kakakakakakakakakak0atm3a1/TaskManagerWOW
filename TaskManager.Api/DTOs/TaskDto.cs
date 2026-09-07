namespace TaskManager.Api.DTOs
{
    public record CreateTaskDto(string Title, string? Description);

    public record TaskResponseDto(int Id, string Title, string? Description, bool IsDone, DateTime CreatedAt);
}