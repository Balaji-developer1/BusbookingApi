using System;
using System.Security.Cryptography;
using System.Text;

namespace BusBookingProjectApi.Services
{
    public static class OtpHelper
    {
        // Generate 6 digit OTP (stronger)
        public static string GenerateOtp()
        {
            byte[] bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            int otp = BitConverter.ToInt32(bytes, 0) % 1000000;
            if (otp < 0) otp = -otp;
            return otp.ToString("D6");
        }

        // Hash OTP with salt
        public static string HashOtp(string otp, string salt)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp + salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Hash password with salt
        public static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Generate random salt
        public static string GenerateSalt()
        {
            var rng = new byte[16];
            RandomNumberGenerator.Fill(rng);
            return Convert.ToBase64String(rng);
        }

        // Verify OTP
        public static bool VerifyOtp(string otp, string salt, string hash)
        {
            var computed = HashOtp(otp, salt);
            return computed == hash;
        }
    }
}
