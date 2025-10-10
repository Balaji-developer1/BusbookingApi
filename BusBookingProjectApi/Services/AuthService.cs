using BusBookingProjectApi.Models;
using BusBookingProjectApi.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepo, IEmailService emailService, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _emailService = emailService;
            _configuration = configuration;
        }

        // 🔹 Register + Send OTP to Email
        public async Task<User> RegisterAsync(string username, string email, string password, string role)
        {
            var existing = await _userRepo.GetByEmailAsync(email);
            if (existing != null)
                throw new Exception("Email already registered");

            // Hash password with salt
            var salt = OtpHelper.GenerateSalt();
            var passwordHash = OtpHelper.HashPassword(password, salt);

            var user = new User
            {
                Username = username,
                Email = email,
                Salt = salt,
                PasswordHash = passwordHash,
                Role = role
            };

            // Save user in DB
            await _userRepo.AddUserAsync(user);

            // Generate OTP & send mail
            var otp = OtpHelper.GenerateOtp();
            await _emailService.SendEmailAsync(email, "Your OTP", $"Your OTP is {otp}");

            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            await _userRepo.UpdateUserAsync(user);

            return user;
        }

        // 🔹 Verify OTP
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return false;
            if (user.OtpExpiry < DateTime.UtcNow) return false;

            if (user.Otp == otp)
            {
                user.IsVerified = true;
                user.Otp = null;
                await _userRepo.UpdateUserAsync(user);
                return true;
            }

            return false;
        }

        // 🔹 Login → Return JWT token
        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null || !user.IsVerified)
                return null;

            var hash = OtpHelper.HashPassword(password, user.Salt);
            if (hash != user.PasswordHash)
                return null;

            return GenerateJwtToken(user);
        }

        // 🔹 Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("userId", user.Id.ToString()),
                // ✅ Use standard ClaimTypes.Role so [Authorize(Roles="Admin")] works
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 🔹 Resend OTP (Optional)
        public async Task<string> GenerateOtpForUserAsync(User user)
        {
            var otp = OtpHelper.GenerateOtp();
            await _emailService.SendEmailAsync(user.Email, "Your OTP", $"Your OTP is {otp}");

            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            await _userRepo.UpdateUserAsync(user);

            return otp;
        }
    }
}
