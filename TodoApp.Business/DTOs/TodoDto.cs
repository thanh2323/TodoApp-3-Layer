using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Data.Models;

namespace TodoApp.Business.DTOs
{
    public class TodoDto
    {
        public int Id { get; set; }
        public string Tile { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public Priority priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string PriorityText => priority.ToString();
        public string StatusText => IsCompleted ? "Completed" : "Pending";
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && !IsCompleted;
        public string DueDateText => DueDate?.ToString("yyyy-MM-dd") ?? "No due date";
    }
}
