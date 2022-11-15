using System;
namespace mycoin.Models
{
    public class LoginResponse
    {
        public int userId { get; set; }
        public int? languageNumber { get; set; }
        public string? deviceNumber { get; set; }
        public string? userName { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public bool result { get; set; }
        public Settings settings { get; set; }
    }
}
