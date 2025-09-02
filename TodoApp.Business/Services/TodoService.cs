using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Business.DTOs;
using TodoApp.Data.Interfaces;
using TodoApp.Data.Models;

namespace TodoApp.Business.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                Priority = todo.Priority,
                DueDate = todo.DueDate,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            };
        }
        public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createDto)
        {
           if(string.IsNullOrWhiteSpace(createDto.Tile))
                throw new ArgumentException("Title cannot be empty");
           if(createDto.DueDate.HasValue && createDto.DueDate.Value < DateTime.Now)
                throw new ArgumentException("Due date cannot be in the past");
            var todo = new Todo
            {
                Title = createDto.Tile.Trim(),
                Description = createDto.Description?.Trim(),
                Priority = createDto.Priority,
                DueDate = createDto.DueDate
            };

            var createdTodo = await _todoRepository.AddAsync(todo);
            return MapToDto(createdTodo);
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
           return await _todoRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
        {
            var todos = await _todoRepository.GetAllTodosAsync();
            return todos.Select(MapToDto);
        }

        public async Task<IEnumerable<TodoDto>> GetFilteredTodosAsync(TodoFilterDto filter)
        {
            IEnumerable<Todo> todos;
            if (filter.ShowOverdue)
            {
                todos = await _todoRepository.GetOverdueTodosAsync();
            }
            else if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                todos = await _todoRepository.SearchAsync(filter.SearchTerm);
            }
            else if (filter.IsCompleted.HasValue)
            {
                todos = await _todoRepository.GetByStatusAsync(filter.IsCompleted.Value);
            }
            else if (filter.Priority.HasValue)
            {
                todos = await _todoRepository.GetByPriorityAsync(filter.Priority.Value);
            }
            else
            {
                todos = await _todoRepository.GetAllTodosAsync();

            }

            if (filter.IsCompleted.HasValue && !filter.ShowOverdue)
            {
                todos = todos.Where(t => t.IsCompleted == filter.IsCompleted.Value);
            }

            if (filter.Priority.HasValue)
            {
                todos = todos.Where(t => t.Priority == filter.Priority.Value);
            }

            return todos.Select(MapToDto);
        }

        public async Task<TodoDto?> GetTodoByIdAsync(int id)
        {
           var todo = await _todoRepository.GetByIdAsync(id);
            return todo == null ? null : MapToDto(todo);
        }

        public async Task<TodoStatsDto> GetTodoStatsAsync()
        {
            var totalCount = await _todoRepository.GetTotalCountAsync();
            var completedCount = await _todoRepository.GetCompletedCountAsync();
            var overdueTodos = await _todoRepository.GetOverdueTodosAsync();

            return new TodoStatsDto
            {
                TotalTodos = totalCount,
                CompletedTodos = completedCount,
                OverdueTodos = overdueTodos.Count()
            };
        }

        public async Task<bool> TodoExistsAsync(int id)
        {
           return await _todoRepository.ExistsAsync(id);
        }

        public async Task<bool> ToggleTodoStatusAsync(int id)
        {
           var todo = await _todoRepository.GetByIdAsync(id);
            if (todo == null)
                return false;
            todo.IsCompleted = !todo.IsCompleted;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateAsync(todo);
            return true;
        }

        public async Task<TodoDto?> UpdateTodoAsync(UpdateTodoDto updateDto)
        {
            var existingTodo = await _todoRepository.GetByIdAsync(updateDto.Id);
            if (existingTodo == null)
                throw new ArgumentException($"Todo With ID {updateDto.Id} Not Found");

            if (string.IsNullOrWhiteSpace(updateDto.Tile))
            {
                throw new ArgumentException("Title cannot be empty");
            }
            if(updateDto.DueDate.HasValue && updateDto.DueDate.Value < DateTime.Now)
            {
                throw new ArgumentException("Due date cannot be in the past for pending todos");
            }
            existingTodo.Title = updateDto.Tile.Trim();
            existingTodo.Description = updateDto.Description?.Trim();
            existingTodo.Priority = updateDto.Priority;
            existingTodo.DueDate = updateDto.DueDate;
            existingTodo.IsCompleted = updateDto.IsCompleted;

            var updatedTodo = await _todoRepository.UpdateAsync(existingTodo);
            return MapToDto(updatedTodo!);
        }
    }
}
