namespace TaskManager.Domain.Entities
{
    public class TaskItem
    {
        public int Id {get; set;}
        public string Title {get; set;} = "unknown";

    public string? Description {get; set;}
    public bool IsDone { get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    }
}