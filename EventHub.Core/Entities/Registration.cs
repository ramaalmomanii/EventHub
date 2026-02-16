using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Entities
{
    public class Registration
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public EEvent Event { get; set; }

        public int AttendeeId { get; set; }
        public User Attendee { get; set; }

        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Cancelled

        // Relations
        public Payment Payment { get; set; }
        public Ticket Ticket { get; set; }
    }
}

