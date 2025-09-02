using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data.Interfaces;
using TodoApp.Data.Models;

namespace TodoApp.Data.Repositories
{
   public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }
        public async Task<Todo> AddAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return false;
            }
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Todos.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            return await _context.Todos
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);

        }

        public async Task<IEnumerable<Todo>> GetByPriorityAsync(Priority priority)
        {
            return await _context.Todos
                .Where(t => t.Priority == priority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetByStatusAsync(bool isCompleted)
        {
            return await _context.Todos
                .Where(t => t.IsCompleted == isCompleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetCompletedCountAsync()
        {
            return await _context.Todos.CountAsync(t => t.IsCompleted);

        }

        public async Task<IEnumerable<Todo>> GetOverdueTodosAsync()
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Todos
                .Where(t => t.DueDate.HasValue && t.DueDate.Value < currentDate && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Todos.CountAsync();
        }

        public async Task<IEnumerable<Todo>> SearchAsync(string searchTerm)
        {
            return await _context.Todos
                .Where(t => t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm)))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Todo?> UpdateAsync(Todo todo)
        {
            todo.UpdatedAt = DateTime.UtcNow;
            _context.Entry(todo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return todo;
        }
    }
}
