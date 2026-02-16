using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Ticket
{
    public class TicketUpdateDto
    {
        public int Id { get; set; }  
        public decimal Price { get; set; }
        public string SeatNumber { get; set; }
    }
}
