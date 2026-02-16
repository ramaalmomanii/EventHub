using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;

namespace EventHub.Core.Repositories
{
    public interface IEventRepository : IGenericRepository<EEvent>
    {
        Task<IEnumerable<EEvent>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<EEvent>> GetByOrganizerAsync(int organizerId);
    }
}

