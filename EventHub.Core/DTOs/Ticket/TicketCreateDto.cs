using EventHub.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Ticket
{
    public class TicketCreateDto
    {
        public int EventId { get; set; }
        public int RegistrationId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public string SeatNumber { get; set; }
    }
}
