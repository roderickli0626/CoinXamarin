using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mycoin.Views
{
    public class MainDashboardPageFlyoutMenuItem
    {
        public MainDashboardPageFlyoutMenuItem()
        {
            TargetType = typeof(MainDashboardPageFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
        public bool IsVisible { get; set; }
    }
}