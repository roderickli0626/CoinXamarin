using System;
using System.Text.RegularExpressions;

namespace mycoin.Extensions
{
    public static class StringExtension
    {
        public static bool ComapreStrings(this string Firststring, string secondstring)
        {
            return (Firststring.ToLower() == secondstring)? true : false;            
        }

        public static bool ValidateEmail(this string emailid)
        {
            bool isValidEmail = Regex.IsMatch(emailid, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isValidEmail;
        }
    }
}
