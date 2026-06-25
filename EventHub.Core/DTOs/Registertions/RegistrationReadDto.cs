using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Registertions
{
    public class RegistrationReadDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public int AttendeeId { get; set; }
        public string AttendeeName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventStatus { get; set; }
    }
}
