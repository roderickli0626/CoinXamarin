using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class Language
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int languageNumber { get; set; }
        public string? description { get; set; }
    }
}
