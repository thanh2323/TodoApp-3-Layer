using Microsoft.AspNetCore.Mvc;
using TodoApp.Business.DTOs;
using TodoApp.Business.Services;
using TodoApp.Web.ViewModels;

namespace TodoApp.Web.Controllers
{
    public class TodoController : Controller
    {
        private readonly ILogger<TodoController> _logger;
        private readonly ITodoService _todoService;
        public TodoController(ILogger<TodoController> logger, ITodoService todoService)
        {
            _logger = logger;
            _todoService = todoService;
        }
        public async Task<IActionResult> Index(TodoFilterViewModel filter)
        {
            try
            {
                var filterDto = new TodoFilterDto
                {
                    IsCompleted = filter.IsCompleted,
                    Priority = filter.Priority,
                    SearchTerm = filter.SearchTerm,
                    ShowOverdue = filter.ShowOverdueOnly
                };
                var todos = await _todoService.GetFilteredTodosAsync(filterDto);
                var stats = await _todoService.GetTodoStatsAsync();

                var viewModel = new TodoIndexViewModel
                {
                    Todos = todos,
                    Filter = filter,
                    Stats = stats
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading todo list");
                TempData["ErrorMessage"] = "An error occurred while loading todos.";
                return View(new TodoIndexViewModel());
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }
               var viewModel = new TodoDetailsViewModel { Todo = todo };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading details for todo id {id}");
                TempData["ErrorMessage"] = "An error occurred while loading the todo details.";
                return RedirectToAction("Index");
            }
        }
        public IActionResult Create()
        {
            return View(new CreateTodoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTodoViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var createDto = new CreateTodoDto
                {
                    Tile = viewModel.Title,
                    Description = viewModel.Description,
                    Priority = viewModel.Priority,
                    DueDate = viewModel.DueDate
                };

                await _todoService.CreateTodoAsync(createDto);
                TempData["SuccessMessage"] = "Todo created successfully.";
                return RedirectToAction(nameof(Index));

            }
            catch (ArgumentException argEx)
            {
                ModelState.AddModelError(string.Empty, argEx.Message);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new todo");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the todo.");
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }
                var viewModel = new EditTodoViewModel
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    Priority = todo.Priority,
                    DueDate = todo.DueDate,
                    IsCompleted = todo.IsCompleted
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading edit form for todo id {id}");
                TempData["ErrorMessage"] = "An error occurred while loading the edit form.";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTodoViewModel viewModel)
        {
            if(id != viewModel.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var updateDto = new UpdateTodoDto
                {
                    Id = viewModel.Id,
                    Tile = viewModel.Title,
                    Description = viewModel.Description,
                    Priority = viewModel.Priority,
                    DueDate = viewModel.DueDate,
                    IsCompleted = viewModel.IsCompleted
                };
                await _todoService.UpdateTodoAsync(updateDto);
                TempData["SuccessMessage"] = "Todo updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException argEx)
            {
                ModelState.AddModelError(string.Empty, argEx.Message);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating todo id {id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the todo.");
                return View(viewModel);
            }

        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if(todo == null)
                {
                    return NotFound();
                }
                var viewModel = new TodoDetailsViewModel { Todo = todo };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading delete confirmation for todo id {id}");
                TempData["ErrorMessage"] = "An error occurred while loading the delete confirmation.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _todoService.DeleteTodoAsync(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Todo not found.";
                }
                else
                {
                   TempData["SuccessMessage"] = "Todo deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting todo id {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the todo.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var success = await _todoService.ToggleTodoStatusAsync(id);
                if (!success)
                {
                    return Json(new {success = false, message = "Todo not found"});
                }
                return Json(new { success = true, message = "Todo status updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling status for todo id {id}");
                return Json(new { success = false, message = "An error occurred while updating the todo status." });
            }
        }
        public async Task<IActionResult> Dashborad()
        {
            try
            {
                var stats = await _todoService.GetTodoStatsAsync();
                var allTodos = await _todoService.GetAllTodosAsync();

                var recentTodos = allTodos.OrderByDescending(t => t.CreatedAt).Take(5);
                var overdueTodos = allTodos.Where(t => t.IsOverdue);
                var highPriorityTodos = allTodos.Where(t => t.Priority == Data.Models.Priority.High && !t.IsCompleted).Take(5);

                var viewModel = new DashboardViewModel
                {
                    Stats = stats,
                    RecentTodos = recentTodos,
                    OverdueTodos = overdueTodos,
                    HighPriorityTodos = highPriorityTodos
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return RedirectToAction("Index");
            }
        }
    }
}
