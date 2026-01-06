using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CrudProject.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrudProject.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class UserApiController : ControllerBase
    {
        private readonly string _filePath;

        public UserApiController(IWebHostEnvironment webHostEnvironment)
        {
            _filePath = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", "users.json");
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
            return roleClaim?.Value ?? "User";
        }

        private List<User> GetUsers()
        {
            try
            {
                if (!System.IO.File.Exists(_filePath)) return new List<User>();
                var json = System.IO.File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json)) return new List<User>();
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch (Exception)
            {
                // In a real app, log this error
                return new List<User>();
            }
        }

        private void SaveUsers(List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            System.IO.File.WriteAllText(_filePath, json);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();
                var allUsers = GetUsers();

                if (currentUserRole == "Admin")
                {
                    // Admins see all regular Users (Role == "User")
                    return Ok(allUsers.Where(u => u.Role == "User").ToList());
                }
                else
                {
                    // Users see only users they created
                    return Ok(allUsers.Where(u => u.CreatedBy == currentUserId).ToList());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving users", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var user = GetUsers().FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user", details = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            try
            {
                if (user == null) return BadRequest("User data is required.");
                if (!ModelState.IsValid) 
                {
                    var errors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(new { message = errors });
                }

                var currentUserId = GetCurrentUserId();
                var users = GetUsers();
                
                // Check if email already exists
                if (users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new { message = "Email already exists" });
                }
                
                user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
                user.Role = "User"; // Dashboard added users are always regular Users
                user.CreatedBy = currentUserId; // Owned by the creator
                user.IsEmailVerified = true; // Dashboard users are auto-verified

                users.Add(user);
                SaveUsers(users);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            try
            {
                if (user == null) return BadRequest("User data is required.");
                if (!ModelState.IsValid) 
                {
                    var errors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(new { message = errors });
                }

                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();
                var users = GetUsers();
                var index = users.FindIndex(u => u.Id == id);
                
                if (index == -1) return NotFound();

                var targetUser = users[index];

                // Permission check
                if (currentUserRole != "Admin" && targetUser.CreatedBy != currentUserId)
                {
                    return Forbid();
                }

                // Check if email already exists (excluding current user)
                if (users.Any(u => u.Id != id && u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Preserve existing sensitive fields if not provided or restricted
                user.Id = id;
                user.Role = targetUser.Role;
                user.CreatedBy = targetUser.CreatedBy;
                user.IsEmailVerified = targetUser.IsEmailVerified;
                user.OTP = targetUser.OTP;
                user.OTPExpiry = targetUser.OTPExpiry;
                
                users[index] = user;
                SaveUsers(users);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();
                var users = GetUsers();
                var user = users.FirstOrDefault(u => u.Id == id);
                
                if (user == null) return NotFound();

                // Permission check
                if (currentUserRole != "Admin" && user.CreatedBy != currentUserId)
                {
                    return Forbid();
                }

                users.Remove(user);
                SaveUsers(users);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting user", details = ex.Message });
            }
        }
    }
}
