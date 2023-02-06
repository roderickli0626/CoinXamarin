using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace mycoin.Models
{
    public class ModuleRes
    {
        public int ModuleID { get; set; }
        public int? GroupNumberID { get; set; }
        public string NameModule { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string ProductNumber { get; set; }
        public string File { get; set; }
        public ImageSource? imageSource { get; set; }
        public string SubscriptionDescription { get; set; }
        public bool? IsSubscription { get; set; }
        public string Color { get; set; }
        public Nullable<System.DateTime> CreatedDatetime { get; set; }
        public Nullable<System.DateTime> UpdatedDatetime { get; set; }
    }
}
