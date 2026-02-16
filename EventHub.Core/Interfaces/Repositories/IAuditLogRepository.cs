using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;

namespace EventHub.Core.Repositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserAsync(int userId);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}

