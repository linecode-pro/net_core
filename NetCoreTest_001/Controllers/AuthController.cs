using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCoreTest_001.Data;
using NetCoreTest_001.Models;

namespace NetCoreTest_001.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TodoContext _context;

        // Конструктор
        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, TodoContext context)
        {
            this.userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // POST: api/Auth
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            IdentityUser user = await userManager.FindByNameAsync(loginModel.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                //return Ok("All is OK");
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    //return Ok("All is OK");
                    // Авторизоваться в приложении под найденным пользователем
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (loginModel.Command == "" || loginModel.Command == "GetTodoList")
                    {
                        return RedirectToRoute(new { controller = "TodoItemsAPI", action = "GetTodoItemsByUserId" });
                    }
                    else if (loginModel.Command == "CreateTodoItem")
                    {
                        if (loginModel.TodoItem == null && loginModel.TodoItem.Name == "")
                        {
                            return BadRequest();
                        }

                        //return RedirectToRoute(new { controller = "TodoItemsAPI", action = "PostTodoItem" , todoItem = loginModel.TodoItem });
                        //return CreatedAtAction("Create", new { todoItem = loginModel.TodoItem });

                        TodoItem todoItem = loginModel.TodoItem;

                        // Получить идентификатор текущего пользователя
                        string userId = userManager.GetUserId(User);

                        // Зафиксировать идентификатор пользователя в свойстве новой задачи
                        todoItem.UserId = userId;

                        // Зафиксировать дату и время создания задачи
                        todoItem.CreationDate = DateTime.Now;

                        _context.Add(todoItem);
                        await _context.SaveChangesAsync();
                        return Ok(todoItem);


                    }
                    else
                    {
                        return BadRequest();
                    }
                    
                }


            }

            return Unauthorized();
        }


        //[HttpGet]
        //public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        //{
        //    _context.TodoItems.Add(todoItem);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        //}

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsComplete,UserId,CreationDate")] TodoItem todoItem)
        {
            if (ModelState.IsValid)
            {
                //// Получить идентификатор пользователя
                //string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //// Получить идентификатор текущего пользователя
                //string userId = userManager.GetUserId(User);

                //// Зафиксировать идентификатор пользователя в свойстве новой задачи
                //todoItem.UserId = userId;

                //// Зафиксировать дату и время создания задачи
                //todoItem.CreationDate = DateTime.Now;

                //_context.Add(todoItem);
                //await _context.SaveChangesAsync();
                //return Ok("All is OK");
            }
            return BadRequest();
        }



    }

    //public class LoginModel
    //{
    //    [Required(ErrorMessage = "Username is required.")]
    //    public string Username { get; set; }
        
    //    [Required(ErrorMessage = "Password is required.")]
    //    public string Password { get; set; }
    //}
}