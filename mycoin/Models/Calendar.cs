using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class Calendar
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string? substanceName { get; set; }
        public byte[]? WavFile { get; set; }
        public DateTime startDate { get; set; }
        public DateTime startTime { get; set; }
        public int Duration { get; set; }
        public bool isRepeat { get; set; }
    }
}
