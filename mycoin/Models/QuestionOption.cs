using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class QuestionOption
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int? UserID { get; set; }
        public string? SelectedQuestionList { get; set; }
    }
}
