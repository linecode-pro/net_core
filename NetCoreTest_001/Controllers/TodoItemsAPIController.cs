using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreTest_001.Data;
using NetCoreTest_001.Models;

namespace NetCoreTest_001.Controllers
{
    //[Authorize]
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsAPIController : ControllerBase
    {
        private readonly TodoContext _context;
        private UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TodoItemsAPIController(TodoContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //// GET: api/TodoItemsAPI
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        //{
        //    return await _context.TodoItems.ToListAsync();
        //}

        // GET: api/TodoItemsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItemsAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
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

        // POST: api/TodoItemsAPI
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            // Получить идентификатор текущего пользователя
            string userId;

            if (User.Identity.IsAuthenticated)  // если пользователь авторизован
            {
                // Найти ползователя по имени
                IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user != null)
                {
                    userId = user.Id;
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }

            // Проверить правильность модели todoItem
            if (!ModelState.IsValid) 
            {
                return BadRequest("Model is not valid!");
            }

            // Проверить заполнение обязательных реквизитов
            if (todoItem.Name == "")
            {
                return BadRequest("Name is required!");
            }

            // Зафиксировать идентификатор пользователя в свойстве новой задачи
            todoItem.UserId = userId;

            // Зафиксировать дату и время создания задачи
            todoItem.CreationDate = DateTime.Now;

            // Создать новую задачу в базе данных
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            // Вернуть новую задачу в качестве результата запроса
            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItemsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        // POST: api/Auth
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            IdentityUser user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                //return Ok("All is OK");
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    //return Ok("All is OK");
                    return RedirectToAction("GetTodoItems");
                }


            }

            return Unauthorized();
        }

        // GET: api/TodoItemsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItemsByUserId()
        {
            // Получить идентификатор текущего пользователя
            //string userId = _userManager.GetUserId(User);

            string userId;

            //return await _context.TodoItems.ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user != null)
                {
                    userId = user.Id;

                    //if (await _signInManager.CanSignInAsync(user))
                    //{


                    //    var result = await _signInManager.SignInAsync(user, isPersistent: false);
                    //    if (result.Succeeded)
                    //    {
                    //        //return Ok("All is OK");
                    //        return RedirectToAction("GetTodoItems");
                    //    }

                    //}
                }
                else
                {
                    return Unauthorized();
                }
                
            }
            else
            {
                return Unauthorized();
            }


            // Вернуть список задач с установленным отбором по UserId
            return await (from a in _context.TodoItems
                               where a.UserId == userId
                               select a).ToListAsync();

        }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public string Command { get; set; } = "";

        public TodoItem TodoItem { get; set; }

    }

}
