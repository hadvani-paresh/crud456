using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User"; // All users are "User" role
        public int CreatedBy { get; set; } = 0;
        
        // Email verification fields
        public string? OTP { get; set; } // OTP sent via email
        public DateTime? OTPExpiry { get; set; } // OTP expiry time
        public bool IsEmailVerified { get; set; } = false; // Email verification status
        
        // Password reset fields
        public string? PasswordResetToken { get; set; } // Password reset token
        public DateTime? PasswordResetExpiry { get; set; } // Password reset token expiry
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class VerifyOTPRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string OTP { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reset token is required.")]
        public string ResetToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
