using Microsoft.AspNetCore.Mvc;
using Models;
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static List<User> _users = new List<User>();
        private static int _nextId = 1;

        // GET: api/User
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return Ok(_users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/User
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            if (!IsValidUser(user, out string validationError))
            {
                return BadRequest(validationError);
            }

            try
            {
                user.Id = _nextId++;
                _users.Add(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!IsValidUser(updatedUser, out string validationError))
            {
                return BadRequest(validationError);
            }

            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                _users.Remove(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to validate user input
        private bool IsValidUser(User user, out string validationError)
        {
            validationError = string.Empty;

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                validationError = "Name cannot be empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !new EmailAddressAttribute().IsValid(user.Email))
            {
                validationError = "Invalid email address.";
                return false;
            }

            return true;
        }
    }
}