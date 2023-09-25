using Microsoft.EntityFrameworkCore;
using TodoList.Api.Models;

namespace TodoList.Api
{
    public class TodoContext : DbContext
    {
        public TodoContext() { }
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
