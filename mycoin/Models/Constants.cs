using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class Constants
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int NewModuleCounts { get; set; }
    }
}
