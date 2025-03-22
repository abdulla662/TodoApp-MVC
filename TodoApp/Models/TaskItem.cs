namespace TodoApp.Models
{
    using System;

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; } 
        public string Priority { get; set; }
        public Boolean? Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
