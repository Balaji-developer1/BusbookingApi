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

        // 🔹 Register + Send OTP
        public async Task<User> RegisterAsync(string username, string email, string password, string role)
        {
            var existing = await _userRepo.GetByEmailAsync(email);
            if (existing != null)
                throw new Exception("Email already registered");

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

            await _userRepo.AddUserAsync(user);

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
            var user = await ValidateUserAsync(email, password);
            if (user == null) return null;

            return GenerateJwtToken(user);
        }

        // 🔹 Validate User for Login
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return null;

            var hash = OtpHelper.HashPassword(password, user.Salt);
            if (hash != user.PasswordHash) return null;

            if (!user.IsVerified) return null;

            return user;
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

        // 🔹 Resend OTP
        public async Task<string> GenerateOtpForUserAsync(User user)
        {
            var otp = OtpHelper.GenerateOtp();
            await _emailService.SendEmailAsync(user.Email, "Your OTP", $"Your OTP is {otp}");

            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            await _userRepo.UpdateUserAsync(user);

            return otp;
        }

        string IAuthService.GenerateJwtToken(User user)
        {
            return GenerateJwtToken(user);
        }
    }
}
