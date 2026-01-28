using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenANew.Models
{
    public class CitizenDashVM
    {
        public int TotalIssues { get; set; }
        public int PendingIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public int EventsJoined { get; set; }
    }
}