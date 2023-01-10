using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class AppSettings
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string? coverSkinDefault { get; set; }
        public string? language { get; set; }
        public int languageNumber { get; set; }
    }
}
