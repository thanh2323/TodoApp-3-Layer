using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Data.Models;

namespace TodoApp.Business.DTOs
{
    public class TodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string PriorityText => Priority.ToString();
        public string StatusText => IsCompleted ? "Completed" : "Pending";
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && !IsCompleted;
        public string DueDateText => DueDate?.ToString("yyyy-MM-dd") ?? "No due date";
    }

    public class CreateTodoDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters" )]
        public string Tile { get; set; } = string.Empty;
        [StringLength(1000,ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "IsCompleted is required")]
        public Priority Priority { get; set; } = Priority.Low;
        [DataType(DataType.DateTime)]
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTodoDto : CreateTodoDto
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TodoFilterDto
    {
        public bool? IsCompleted { get; set; }
        public Priority? Priority { get; set; }

        public string? SearchTerm { get; set; }

        public bool ShowOverdue { get; set; } = false;
    }

    public class TodoStatsDto
    {
        public int TotalTodos { get; set; }
        public int CompletedTodos { get; set; }
        public int PendingTodos => TotalTodos - CompletedTodos;
        public int OverdueTodos { get; set; }

        public double CompletionRate => TotalTodos == 0 ? 0 : (double)CompletedTodos / TotalTodos * 100;
    }
}
