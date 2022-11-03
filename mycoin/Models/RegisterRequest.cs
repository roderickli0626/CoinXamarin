using System;
namespace mycoin.Models
{
    public class RegisterRequest
    {
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string serialNumber { get; set; }
        public string language { get; set; }
    }
}
