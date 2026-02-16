using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Testimonials
{
    public class TestimonialCreateDto
    {
        public int AttendeeId { get; set; }
        public int EventId { get; set; }
        public string Content { get; set; }
    }

   
}

