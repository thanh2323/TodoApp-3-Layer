using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TodoApp.Business.DTOs;
using TodoApp.Data.Models;

namespace TodoApp.Web.ViewModels
{
    public class TodoIndexViewModel
    {
        public IEnumerable<TodoDto> Todos { get; set; } = new List<TodoDto>();
        public TodoFilterViewModel Filter { get; set; } = new TodoFilterViewModel();

        public TodoStatsDto Stats { get; set; } = new TodoStatsDto();

        public string CurrentFilter => GetCurrentFilterText();

        private string GetCurrentFilterText()
        {
            var filters = new List<string>();

            if(Filter.IsCompleted.HasValue)
            {
                filters.Add(Filter.IsCompleted.Value ? "Completed" : "Pending");
            }
            if (Filter.Priority.HasValue)
            {
                filters.Add(Filter.Priority.Value.ToString());
            }
            if (!string.IsNullOrWhiteSpace(Filter.SearchTerm))
            {
                filters.Add($"Search: '{Filter.SearchTerm}'");
            }
            if (Filter.ShowOverdueOnly)
            {
                filters.Add("Overdue");
            }
            return filters.Any() ? string.Join(", ", filters) : "All Todos";
        }
    }

    public class TodoFilterViewModel
    {
        public bool? IsCompleted { get; set; }
        public Priority? Priority { get; set; }
        [Display(Name = "Search")]
        [StringLength(100)]
        public string? SearchTerm { get; set; }

        [Display(Name = "Show Overdue Only")]
        public bool ShowOverdueOnly { get; set; } = false;

        public SelectList StatusOptions => new SelectList(new[]
        {
            new { Value = "", Text = "All Status" },
            new { Value = "false", Text = "Pending" },
            new { Value = "true", Text = "Completed" }
        }, "Value", "Text", IsCompleted?.ToString().ToLower());

        public SelectList PriorityOptions => new SelectList(new[]
        {
            new { Value = "", Text = "All Priorities" },
            new { Value = "1", Text = "Low" },
            new { Value = "2", Text = "Medium" },
            new { Value = "3", Text = "High" }
        }, "Value", "Text", Priority?.ToString("D"));
    }
    public class CreateTodoViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public Priority Priority { get; set; } = Priority.Low;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public SelectList PriorityOptions => new SelectList(Enum.GetValues<Priority>()
            .Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text");
    }

    public class EditTodoViewModel : CreateTodoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class TodoDetailsViewModel
    {
        public TodoDto Todo { get; set; } = new TodoDto();
        public string FormattedCreatedAt => Todo.CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedUpdatedAt => Todo.UpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Never";
        public string PriorityBadgeClass => Todo.Priority switch
        {
            Priority.High => "badge-danger",
            Priority.Medium => "badge-warning",
            Priority.Low => "badge-success",
            _ => "badge-secondary"
        };
        public string StatusBadgeClass => Todo.IsCompleted ? "badge-success" : "badge-secondary";
    }

    public class DashboardViewModel
    {
        public TodoStatsDto Stats { get; set; } = new TodoStatsDto();
        public IEnumerable<TodoDto> RecentTodos { get; set; } = new List<TodoDto>();
        public IEnumerable<TodoDto> OverdueTodos { get; set; } = new List<TodoDto>();
        public IEnumerable<TodoDto> HighPriorityTodos { get; set; } = new List<TodoDto>();
    }
}
