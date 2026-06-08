using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Events
{
    public class EventReadDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string OrganizerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}
