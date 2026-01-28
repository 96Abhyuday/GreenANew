using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenANew.Models
{
    public class DashBoardVM
    {
        public User User { get; set; }
        public List<Issue> Issues { get; set; }
        public List<task> MyTasks { get; set; }
        public List<PlantationEvent> Events { get; set; }
        public List<EventParticipant> MyEventParticipations { get; set; }
    }
}