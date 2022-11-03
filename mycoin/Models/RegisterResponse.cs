using System;
namespace mycoin.Models
{
    public class RegisterResponse
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public bool result { get; set; }
    }
}
