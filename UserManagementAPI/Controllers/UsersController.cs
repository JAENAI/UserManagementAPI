using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> Users = [];
    private static readonly object UsersLock = new();
    private static int nextId = 1;

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        lock (UsersLock)
        {
            return Ok(Users.ToList());
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetUser(int id)
    {
        lock (UsersLock)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            return user is null ? NotFound() : Ok(user);
        }
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        lock (UsersLock)
        {
            user.Id = nextId++;
            Users.Add(user);
        }

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        lock (UsersLock)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            if (user is null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        lock (UsersLock)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            if (user is null)
            {
                return NotFound();
            }

            Users.Remove(user);
        }

        return NoContent();
    }
}