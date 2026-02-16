using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Core.Repositories;
using EventHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EventHub.Infrastructure.Repositories
{
    public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository
    {
        private readonly EventHubDbContext _context;

        public RegistrationRepository(EventHubDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Registration?> GetByUserAndEventAsync(int userId, int eventId)
        {
            return await _context.Registrations
        .FirstOrDefaultAsync(r => r.AttendeeId == userId && r.EventId == eventId);
        }

        public async Task<IEnumerable<Registration>> GetByUserAsync(int userId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.AttendeeId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Registration>> GetByEventAsync(int eventId)
        {
            return await _context.Registrations
                .Include(r => r.Attendee)
                .Where(r => r.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Registration>> FindByUserAsync(int userId)
        {
            return await _context.Registrations
                                 .Where(r => r.AttendeeId == userId)
                                 .ToListAsync();
        }


        public async Task<Registration?> GetWithDetailsAsync(int userId, int eventId)
        {
            return await _context.Registrations
                .Include(r => r.Attendee)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == userId && r.EventId == eventId);
        }




    }


}
