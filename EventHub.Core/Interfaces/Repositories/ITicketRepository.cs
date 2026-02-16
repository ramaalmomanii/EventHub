using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;

namespace EventHub.Core.Repositories
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket?> GetByUserAndEventAsync(int userId, int eventId);
        Task<Ticket?> GetByQrCodeAsync(string qrCode);

        Task<IEnumerable<Ticket>> GetByUserAsync(int userId);
        Task<Ticket?> GetWithDetailsAsync(int ticketId);
    }
}

