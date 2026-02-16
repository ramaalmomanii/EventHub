using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Testimonials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Interfaces.Services
{
    public interface ITestimonialService : IGenericService<TestimonialReadDto>
    {
        Task<IEnumerable<TestimonialReadDto>> GetByStatusAsync(string status);
    }
}

