using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Infrastructure.Data;
using EventHub.Core.Repositories;
using EventHub.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Infrastructure.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly EventHubDbContext _context;

        public TicketRepository(EventHubDbContext context) : base(context)
        {
            _context = context;
        }

        // Get ticket by User + Event
        public async Task<Ticket?> GetByUserAndEventAsync(int userId, int eventId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.UserId)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == eventId);
        }

        // Get all tickets of a specific user
        public async Task<IEnumerable<Ticket>> GetByUserAsync(int userId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.Attendee)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public override async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Attendee)
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == id);
        }


        // Get ticket with event + user (full details)
        public async Task<Ticket?> GetWithDetailsAsync(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.UserId)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        // Get ticket by QR code
        public async Task<Ticket?> GetByQrCodeAsync(string qrCode)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.UserId)
                .FirstOrDefaultAsync(t => t.QRCode == qrCode);
        }
    }
}
