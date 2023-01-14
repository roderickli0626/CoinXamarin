using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class Question
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int? GroupNumber { get; set; }
        public int? LanguageNumber { get; set; }
        public string? QuestionContent { get; set; }
        public string? QuestionTitle { get; set; }
    }
}
