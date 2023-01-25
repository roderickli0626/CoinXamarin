using System;
namespace mycoin.Helpers
{
    public static class ApiURLs
    {
        public static readonly string Login = "Auth/Login";
        public static readonly string Register = "Auth/Register";
        public static readonly string ForgotPassword = "Auth/ForgotPasswordRequest";
        public static readonly string LoadDB = "Substance/loadDB";
        public static readonly string LoadAllDB = "Substance/loadAllDB";
        public static readonly string LoadLanguages = "Language/loadDB";
        public static readonly string LoadLanguageGUI = "LanguageGUI/loadDB";
        public static readonly string LoadSubstanceText = "SubstanceText/loadDB";
        public static readonly string LoadSubstanceGroupText = "SubstanceGroupText/loadDB";
        public static readonly string LoadQuestions = "Question/loadDB";
    }
}
