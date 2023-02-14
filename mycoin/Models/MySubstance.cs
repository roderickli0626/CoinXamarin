using System;
using System.Collections.Generic;
using System.Text;

namespace mycoin.Models
{
    public class MySubstance
    {
        public int ID { get; set; }
        public string SubstanceImageUrl { get; set; }
        public string SubstanceName { get; set; }
        public int Duration { get; set; }
        public string DurationTimeFormat { get; set; }
        public bool favoriteExtraIconVisible { get; set; }
        public DateTime? PlayDateTime { get; set; }
    }
}
