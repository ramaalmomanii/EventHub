using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;
using EventHub.Core.Repositories;
using EventHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly EventHubDbContext _context;

        public PaymentRepository(EventHubDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetByUserAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.Registration)
                .ThenInclude(r => r.Event)
                .Where(p => p.Registration.AttendeeId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByEventAsync(int eventId)
        {
            return await _context.Payments
                .Include(p => p.Registration)
                .ThenInclude(r => r.Event)
                .Where(p => p.Registration.EventId == eventId)
                .ToListAsync();
        }

        public async Task<Registration?> GetByIdWithEventAsync(int id)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}