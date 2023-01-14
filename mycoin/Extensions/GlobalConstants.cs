using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Extensions
{
    public static class GlobalConstants
    {
        //Constants For Calendars
        public static int Duration { get; set; }
        public static bool RunFlag { get; set; }
        public static DateTime StartTime { get; set; }

        //Constants For Setting / Language
        public static Dictionary<int, string> SubTexts { get; set; } = new Dictionary<int, string>();
        public static Dictionary<int, string> GroupTexts { get; set; } = new Dictionary<int, string>();
        public static Dictionary<string, string> LangGUI { get; set; } = new Dictionary<string, string>();

        //Constant For Questions
        public static List<Question> Questions { get; set; } = new List<Question>();
        public static int QuestionPageNumber { get; set; }
        public static List<int> GroupIds { get; set; } = new List<int>();

    }
}
