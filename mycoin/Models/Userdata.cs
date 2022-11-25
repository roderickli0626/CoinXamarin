using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class Userdata
    {
        public int userid { get; set; }
        public int? languageid { get; set; }
        public string devicenum { get; set; }
        public string? userName { get; set; }

        //For 'Remember Me' check
        public string email { get; set; }
        public string password { get; set; } 
        public bool isActive { get; set; }
    }
}
