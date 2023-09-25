using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TodoList.Api.Controllers;
using TodoList.Api.DTOs;
using TodoList.Api.Models;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsControllerTests
    {
        private readonly TodoItemsController _controller;
        
        public TodoItemsControllerTests()
        {
            _controller = new TodoItemsController(GetDatabaseContext(), new Mock<ILogger<TodoItemsController>>().Object);
        }

        #region "Setup and Database Context"
        private TodoContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new TodoContext(options);
            databaseContext.Database.EnsureCreated();
            databaseContext.TodoItems.Add(new TodoItem()
            {
                Id = new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e"),
                Description = "111",
                IsCompleted = false
            });
            databaseContext.TodoItems.Add(new TodoItem()
            {
                Id = new Guid("57280354-aa45-4791-a0c0-3ffeb3d11a73"),
                Description = "222",
                IsCompleted = false
            });
            databaseContext.SaveChangesAsync();

            return databaseContext;
        }
        #endregion

        #region "Get TodoItems"

        [Fact(DisplayName = "Check list of all TodoItems")]
        public void GetToDoItems_Test_ListofToDoItem()
        {
            //Arrange
            var expected = new List<TodoItemDTO>
            {
                new TodoItemDTO { Id = new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e"), Description = "111", IsCompleted = false },
                new TodoItemDTO { Id = new Guid("57280354-aa45-4791-a0c0-3ffeb3d11a73"), Description = "222", IsCompleted = false }
            };

            //Act
            var result = _controller.GetTodoItems().Result;
            var actual = result.Value as List<TodoItemDTO>;

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected.Count(), actual.Count());
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        [Fact(DisplayName = "Check to find TodoItem by Guid")]
        public void GetToDoItem_Test_FindofToDoItem()
        {
            //Arrange
            var expected = new TodoItemDTO { Id = new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e"), Description = "111", IsCompleted = false };

            //Act
            var result = _controller.GetTodoItem(new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e")).Result as OkObjectResult;
            var actual = result.Value as TodoItemDTO;

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        [Fact(DisplayName = "Check to find TodoItem by Guid not in the list")]
        public void GetToDoItem_Test_FindofToDoItemNotFound()
        {
            //Arrange
            var httpStatusCode = new NotFoundResult();

            //Act
            var result = _controller.GetTodoItem(new Guid()).Result as NotFoundResult;

            //Assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(result.StatusCode, httpStatusCode.StatusCode);
        }

        #endregion

        #region "Create ToDoItems"

        [Fact(DisplayName = "Create ToDoItem Where Description is not given but required")]
        public void PostTodoItem_Test_DescriptionIsNotGiven()
        {
            //Arrange
            var httpStatusCode = new BadRequestObjectResult("Description is required");
            TodoItemDTO list = new TodoItemDTO { Id = Guid.NewGuid(), IsCompleted = false };

            //Act
            var result = _controller.PostTodoItem(list).Result;

            //Assert
            Assert.Equal(((BadRequestObjectResult)result).Value,httpStatusCode.Value);
        }

        [Fact(DisplayName = "Create ToDoItem Where Description is given but already exists")]
        public void PostTodoItem_Test_DescriptionAlreadyExists()
        {
            //Arrange
            var httpStatusCode = new BadRequestObjectResult("Description already exists");
            TodoItemDTO list = new TodoItemDTO { Id = Guid.NewGuid(), Description="111", IsCompleted = false };

            //Act
            var result = _controller.PostTodoItem(list).Result;

            //Assert
            Assert.Equal(((BadRequestObjectResult)result).Value, httpStatusCode.Value);
        }

        #endregion

        #region "Update ToDoItem"

        [Fact(DisplayName = "Update ToDoItem Where Id does not exist")]
        public void PutTodoItem_Test_UpdateNotExistingToDoItem()
        {
            //Arrange
            var httpStatusCode = new BadRequestResult();
            
            //Act
            var getResult = _controller.GetTodoItem(new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e")).Result as OkObjectResult;
            var result = _controller.PutTodoItem(Guid.NewGuid(), getResult.Value as TodoItemDTO).Result;

            //Assert
            Assert.Equal(((BadRequestResult)result).StatusCode, httpStatusCode.StatusCode);
        }

        [Fact(DisplayName = "Update ToDoItem Where Id exists")]
        public void PutTodoItem_Test_ExistingToDoItem()
        {
            //Arrange
            var httpStatusCode = new NoContentResult();

            //Act
            var getResult = _controller.GetTodoItem(new Guid("57280354-aa45-4791-a0c0-3ffeb3d11a73")).Result as OkObjectResult;
            var result = _controller.PutTodoItem(new Guid("57280354-aa45-4791-a0c0-3ffeb3d11a73"), getResult.Value as TodoItemDTO).Result;

            //Assert
            Assert.Equal(((NoContentResult)result).StatusCode, httpStatusCode.StatusCode);
        }

        //[Fact(DisplayName = "Update ToDoItem Where Id exists")]
        //public void PutTodoItem_Test_ExistingMultipleToDoItem()
        //{
        //    DbUpdateConcurrencyException exception = new DbUpdateConcurrencyException();
        //    var getResult = _controller.GetTodoItem(new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e")).Result as OkObjectResult;

        //    var result = _controller.PutTodoItem(new Guid("f03a7d8d-22c5-4ee4-bc1c-1e073d9a103e"), getResult.Value as TodoItem).Result as DbUpdateConcurrencyException;
        //    Assert.Equal(result, exception);
        //}

        #endregion

        #region Delete ToDoItems

        [Fact(DisplayName = "Delete TodoItem by Guid not in the list")]
        public void DeleteToDoItem_Test_FindofToDoItemNotFound()
        {
            //Arrange
            var httpStatusCode = new NotFoundResult();

            //Act
            var result = _controller.DeleteTodoItem(new Guid()).Result as NotFoundResult;

            //Assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(result.StatusCode, httpStatusCode.StatusCode);
        }

        [Fact(DisplayName = "Delete TodoItem by Guid  in the list")]
        public void DeleteToDoItem_Test_FromTheToDoItem()
        {
            //Arrange
            var httpStatusCode = new NoContentResult();

            //Act
            var getResult = _controller.DeleteTodoItem(new Guid("57280354-aa45-4791-a0c0-3ffeb3d11a73")).Result as NoContentResult;

            //Assert
            Assert.IsType<NoContentResult>(getResult);
            Assert.Equal(getResult.StatusCode, httpStatusCode.StatusCode);
        }
        #endregion

       
    }
}
