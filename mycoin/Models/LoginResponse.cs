using System;
namespace mycoin.Models
{
    public class LoginResponse
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public bool result { get; set; }
        public Settings settings { get; set; }
    }
}
