using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.DTOs;

namespace EventHub.Core.Interfaces.Services
{
    public interface IAuditLogService : IGenericService<AuditLogReadDto>
    {
        Task<IEnumerable<AuditLogReadDto>> GetByUserAsync(int userId);
        Task<IEnumerable<AuditLogReadDto>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}
