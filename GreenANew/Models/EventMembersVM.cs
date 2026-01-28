using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenANew.Models
{
    public class EventMembersVM
    {
        public PlantationEvent Event { get; set; }
        public List<EventParticipant> Participants { get; set; }
    }
}