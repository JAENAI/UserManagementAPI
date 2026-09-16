using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly Dictionary<int, User> Users = [];
    private static readonly object UsersLock = new();
    private static int nextId = 1;

    private readonly ILogger<UsersController> logger;

    public UsersController(ILogger<UsersController> logger)
    {
        this.logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        try
        {
            lock (UsersLock)
            {
                return Ok(Users.Values.ToArray());
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve users.");
            return Problem("The users could not be retrieved.");
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetUser(int id)
    {
        if (id <= 0)
        {
            return BadRequest("User ID must be greater than zero.");
        }

        try
        {
            lock (UsersLock)
            {
                return Users.TryGetValue(id, out var user) ? Ok(user) : NotFound();
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve user {UserId}.", id);
            return Problem("The user could not be retrieved.");
        }
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        try
        {
            lock (UsersLock)
            {
                user.Id = nextId++;
                Users.Add(user.Id, user);
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to create user.");
            return Problem("The user could not be created.");
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {
        if (id <= 0)
        {
            return BadRequest("User ID must be greater than zero.");
        }

        try
        {
            lock (UsersLock)
            {
                if (!Users.TryGetValue(id, out var user))
                {
                    return NotFound();
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to update user {UserId}.", id);
            return Problem("The user could not be updated.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        if (id <= 0)
        {
            return BadRequest("User ID must be greater than zero.");
        }

        try
        {
            lock (UsersLock)
            {
                if (!Users.Remove(id))
                {
                    return NotFound();
                }
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to delete user {UserId}.", id);
            return Problem("The user could not be deleted.");
        }

        return NoContent();
    }
}