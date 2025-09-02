using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Business.DTOs;

namespace TodoApp.Business.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoDto>> GetAllTodosAsync();
        Task<TodoDto?> GetTodoByIdAsync(int id);
        Task<IEnumerable<TodoDto>> GetFilteredTodosAsync(TodoFilterDto filter);
        Task<TodoDto> CreateTodoAsync(CreateTodoDto createDto);
        Task<TodoDto?> UpdateTodoAsync(UpdateTodoDto updateDto);
        Task<bool> DeleteTodoAsync(int id);
        Task<TodoStatsDto> GetTodoStatsAsync();

        Task<bool> ToggleTodoStatusAsync(int id);

        Task<bool> TodoExistsAsync(int id);
    }
}
