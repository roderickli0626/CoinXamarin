using System;
using SQLite;

namespace mycoin.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int SubstanceID { get; set; }
        public bool? Hidde { get; set; }
        public byte[]? WavFile { get; set; }
        public bool? StandardYesNo { get; set; }
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Adress { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? DeviceNumber { get; set; }
        public int? LanguageNumber { get; set; }
        public bool? ActiveAcount { get; set; }
        public string? GroupName { get; set; }
        public string? Substance { get; set; }
        public string? Language { get; set; }
        public bool Isfavorite { get; set; }
        public DateTime DefaultDateTime { get; set; }
        public int Duration { get; set; }

    }
}
