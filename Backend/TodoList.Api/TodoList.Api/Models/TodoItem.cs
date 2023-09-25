using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; }

        [ConcurrencyCheck]
        public string Description { get; set; }

        public string Priority { get; set; }

        public bool IsCompleted { get; set; }
    }
}
