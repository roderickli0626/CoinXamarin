using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mycoin.Models
{
    public class MyGroup
    {
        public string ImageUrl { get; set; }
        public string GroupName { get; set; }
        public List<MySubstance> MySubstances { get; set; } = new List<MySubstance>();
    }
}
