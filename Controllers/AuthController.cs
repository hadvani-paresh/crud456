using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using CrudProject.Models;
using CrudProject.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CrudProject.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly string _filePath;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IEmailService emailService)
        {
            _filePath = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", "users.json");
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"[LOGIN ATTEMPT] Email: {request.Email}");
            
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password && u.IsEmailVerified);

            if (user == null)
            {
                Console.WriteLine($"[LOGIN FAILED] Invalid credentials for: {request.Email}");
                return Unauthorized(new { message = "Invalid email or password, or email not verified" });
            }

            Console.WriteLine($"[LOGIN SUCCESS] User: {user.Email}, Role: {user.Role}");

            var token = GenerateJwtToken(user);

            // Also sign in to Cookie scheme for MVC page protection
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email), // Use name if available, fallback to email
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            Console.WriteLine($"[LOGIN COMPLETE] Token generated and cookie set for: {user.Email}");

            return Ok(new { token, user = new { user.Id, user.Name, user.Email, user.Role } });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null) return BadRequest("User data is required.");
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { message = errors });
            }

            var users = GetUsers();
            if (users.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already registered" });
            }

            // Generate OTP
            var otp = GenerateOTP();

            var user = new User
            {
                Id = users.Any() ? users.Max(u => u.Id) + 1 : 1,
                Name = request.Email.Split('@')[0], // Use email prefix as name initially
                Email = request.Email,
                Password = request.Password,
                Role = "User", // All users are "User" role
                CreatedBy = 0,
                OTP = otp,
                OTPExpiry = DateTime.UtcNow.AddMinutes(10), // OTP expires in 10 minutes
                IsEmailVerified = false,
                PasswordResetToken = null,
                PasswordResetExpiry = null
            };

            users.Add(user);
            SaveUsers(users);

            // Send OTP via email
            var emailSent = await _emailService.SendOTPAsync(user.Email, otp);
            
            if (!emailSent)
            {
                return StatusCode(500, new { message = "Registration successful but failed to send OTP email. Please contact support." });
            }

            return Ok(new { 
                message = "Registration successful! Please check your email for OTP.",
                email = user.Email 
            });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Email == request.Email && !u.IsEmailVerified);

            if (user == null)
            {
                return BadRequest(new { message = "User not found or already verified" });
            }

            if (user.OTPExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "OTP has expired. Please register again." });
            }

            if (user.OTP != request.OTP)
            {
                return BadRequest(new { message = "Invalid OTP" });
            }

            // Mark user as verified and clear OTP
            user.IsEmailVerified = true;
            user.OTP = null;
            user.OTPExpiry = null;

            SaveUsers(users);

            return Ok(new { message = "Email verified successfully! You can now login." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { message = errors });
            }

            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return Ok(new { message = "If your email is registered, you will receive a password reset code." });
            }

            // Generate reset token (6-digit code)
            var resetToken = GenerateResetToken();

            // Update user with reset token and expiry
            user.PasswordResetToken = resetToken;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15); // 15 minutes expiry

            SaveUsers(users);

            // Send reset email
            var emailSent = await _emailService.SendPasswordResetAsync(user.Email, resetToken);
            
            if (!emailSent)
            {
                return StatusCode(500, new { message = "Failed to send password reset email. Please try again." });
            }

            return Ok(new { 
                message = "If your email is registered, you will receive a password reset code.",
                email = user.Email 
            });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { message = errors });
            }

            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or reset token." });
            }

            // Check if reset token is valid and not expired
            if (string.IsNullOrEmpty(user.PasswordResetToken) || 
                user.PasswordResetToken != request.ResetToken ||
                user.PasswordResetExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired reset token." });
            }

            // Update password and clear reset token
            user.Password = request.NewPassword;
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;

            SaveUsers(users);

            return Ok(new { message = "Password has been reset successfully. You can now login with your new password." });
        }

        private string GenerateResetToken()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit reset token
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }

        private void SaveUsers(List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            System.IO.File.WriteAllText(_filePath, json);
        }

        private List<User> GetUsers()
        {
            if (!System.IO.File.Exists(_filePath)) return new List<User>();
            var json = System.IO.File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyForJWTAuthentication12345");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name ?? user.Email), // Use name if available
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "CrudProject",
                Audience = "CrudProjectUsers",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
