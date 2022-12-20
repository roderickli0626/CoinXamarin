using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class SubstanceText
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int? SubstanceID { get; set; }
        public string? Description { get; set; }
        public int? Language { get; set; }
    }
}
