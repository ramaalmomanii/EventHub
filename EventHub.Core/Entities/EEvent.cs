using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Entities
{
    public class EEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }


        public int OrganizerId { get; set; }
        public User Organizer { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } ="Pending";// Upcoming, Ongoing, Completed, Cancelled
        public DateTime CreatedAt { get; set; }

        // Relations
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<Testimonial> Testimonials { get; set; }
    }
}

