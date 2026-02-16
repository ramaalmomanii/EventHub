using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;

namespace EventHub.Core.Repositories
{
    public interface ITestimonialRepository : IGenericRepository<Testimonial>
    {
        Task<IEnumerable<Testimonial>> GetByStatusAsync(string status);
    }
}

