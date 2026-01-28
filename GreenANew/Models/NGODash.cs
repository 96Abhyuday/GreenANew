using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenANew.Models
{
    public class NGODash
    {
        public int AssignedTo { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int UserCount { get; set; }
    }
}