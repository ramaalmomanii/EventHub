using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Ticket
{
    public record TicketReadDto
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public string PdfPath { get; set; }

        public string SeatNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        public string EventTitle { get; set; } = string.Empty;
        public DateTime? EventStartDate { get; set; }
        public string? EventLocation { get; set; }
    }
}

