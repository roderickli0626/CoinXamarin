using System;
namespace mycoin.Models
{
    public class Settings
    {
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string email { get; set; }
        public bool optOutFromDataCollection { get; set; }
        public string connectedCoin { get; set; }
        public string coinCover { get; set; }
        public string language { get; set; }
    }
}
