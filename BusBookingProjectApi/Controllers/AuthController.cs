using BusBookingProjectApi.Models;
using BusBookingProjectApi.Repositories;
using BusBookingProjectApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepo;

        public AuthController(IAuthService authService, IUserRepository userRepo)
        {
            _authService = authService;
            _userRepo = userRepo;
        }

        // 🔹 Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                var user = await _authService.RegisterAsync(model.Username, model.Email, model.Password, model.Role);
                return Ok(new { message = "User registered successfully, OTP sent to email", userId = user.Id });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 🔹 Verify OTP
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest model)
        {
            var success = await _authService.VerifyOtpAsync(model.Email, model.Otp);
            if (success) return Ok(new { message = "OTP verified, account activated" });
            return BadRequest(new { error = "Invalid or expired OTP" });
        }

        // 🔹 Login → returns JWT token + Role
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _authService.ValidateUserAsync(model.Email, model.Password);
            if (user == null)
                return Unauthorized(new { error = "Invalid credentials or account not verified" });

            // 🔹 Generate JWT token
            
            var token = _authService.GenerateJwtToken(user);


            return Ok(new
            {
                token,
                role = user.Role,       // Admin/User check
                email = user.Email
            });
        }

        // 🔹 Resend OTP
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest model)
        {
            var user = await _userRepo.GetByEmailAsync(model.Email);
            if (user == null) return BadRequest(new { error = "User not found" });

            var otp = await _authService.GenerateOtpForUserAsync(user);
            return Ok(new { message = "OTP resent to email" });
        }
    }

    // 🔹 Request DTOs
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // User/Admin
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ResendOtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
