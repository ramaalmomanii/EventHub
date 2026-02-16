using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;

namespace EventHub.Core.Repositories
{
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        Task<Registration?> GetByUserAndEventAsync(int userId, int eventId);
        Task<IEnumerable<Registration>> GetByUserAsync(int userId);
        Task<IEnumerable<Registration>> GetByEventAsync(int eventId);
        Task<Registration?> GetWithDetailsAsync(int userId, int eventId);

    }
}

