using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class LanguageGUI
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string? key { get; set; }
        public string? content { get; set; }
        public int LanguageNumber { get; set; }
    }
}
