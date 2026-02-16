using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Entities
{
    public class Testimonial
    {
        public int Id { get; set; }

        public int AttendeeId { get; set; }
        public User Attendee { get; set; }

        public int EventId { get; set; }
        public EEvent Event { get; set; }

        public string Content { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
    }
}

