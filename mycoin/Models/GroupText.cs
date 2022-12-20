using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class GroupText
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int? GroupNumber { get; set; }
        public int? Language { get; set; }
        public string? Description { get; set; }
    }
}
