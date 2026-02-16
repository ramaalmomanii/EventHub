using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Events
{
    public class EventCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
       public int categoryId { get; set; }
        public int OrganizerId { get; set;  }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = "Pending";
    }

    

    
}

