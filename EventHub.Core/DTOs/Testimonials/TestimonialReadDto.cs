using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Testimonials
{
    public class TestimonialReadDto
    {
        public int Id { get; set; }
        public string AttendeeName { get; set; }
        public string EventTitle { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
