using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Data.Models;

namespace TodoApp.Data.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetAllTodosAsync();
        Task<Todo?> GetByIdAsync(int id);

        Task<IEnumerable<Todo>> GetByStatusAsync(bool isCompleted);

        Task<IEnumerable<Todo>> GetByPriorityAsync(Priority priority);

        Task<IEnumerable<Todo>> SearchAsync(string searchTerm);

        Task<Todo> AddAsync(Todo todo);
        Task<Todo?> UpdateAsync(Todo todo);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<int> GetTotalCountAsync();

        Task<int> GetCompletedCountAsync();

        Task<IEnumerable<Todo>> GetOverdueTodosAsync();
    }
}
