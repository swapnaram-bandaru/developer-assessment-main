using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.DTOs;
using TodoList.Api.Models;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            var results= await _context.TodoItems.Where(x => !x.IsCompleted).Select(m => MapFromToDoItemToToDoItemDTO(m)).ToListAsync();
             return Ok(results);
            //return results;
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            var result = await _context.TodoItems.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(MapFromToDoItemToToDoItemDTO(result));
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItemDTO todoItemdto)
        {
            if (id != todoItemdto.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem != null)
            {
                todoItem.Description = todoItemdto.Description;
                todoItem.IsCompleted = todoItemdto.IsCompleted;
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemIdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems 
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItemDTO todoItemdto)
        {
            if (string.IsNullOrEmpty(todoItemdto?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (TodoItemDescriptionExists(todoItemdto.Description))
            {
                return BadRequest("Description already exists");
            }

            var todoItem = MapFromToDoItemDTOToToDoItem(todoItemdto);

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, MapFromToDoItemToToDoItemDTO(todoItem));
        }

        // DELETE: api/TodoItems1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemIdExists(Guid id)
        {
            return _context.TodoItems.Any(x => x.Id == id);
        }

        private bool TodoItemDescriptionExists(string description)
        {
            var v = _context.TodoItems
                   .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
            return v;
        }

        private static TodoItemDTO MapFromToDoItemToToDoItemDTO(TodoItem todoItem)
        {
            return new TodoItemDTO
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted
            };
        }

        private static TodoItem MapFromToDoItemDTOToToDoItem(TodoItemDTO todoItemdto)
        {
            return new TodoItem
            {
                Id = todoItemdto.Id,
                Description = todoItemdto.Description,
                IsCompleted = todoItemdto.IsCompleted
            };
        }
    }
}
