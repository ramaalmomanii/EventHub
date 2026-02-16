using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public EEvent Event { get; set; }

        public int RegistrationId { get; set; }      
        public Registration Registration { get; set; }


        public int UserId { get; set; }
        public User Attendee { get; set; }

        public decimal Price { get; set; }
        public string SeatNumber { get; set; }

        public string? QRCode { get; set; }
        public string? PdfPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       // public string? Status { get; set; }
    }
}

