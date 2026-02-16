using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Registertions
{
    public class RegistrationCreateDto
    {
        
        public int EventId { get; set; }

       // public int AttendeeId { get; set; }
        public DateTime RegistrationDate { get; set; }= DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
    }

}

   

