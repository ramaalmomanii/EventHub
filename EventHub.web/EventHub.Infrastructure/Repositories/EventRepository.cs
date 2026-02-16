using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Infrastructure.Data;
using EventHub.Core.Repositories;
using EventHub.Infrastructure.Repositories;
using EventHub.Core.Entities;
using Microsoft.EntityFrameworkCore;
using EventHub.Core.Interfaces;


namespace EventHub.Infrastructure.Repositories
{
    public class EventRepository :GenericRepository<EEvent>, IEventRepository
    {
        private readonly EventHubDbContext _context;
        public EventRepository(EventHubDbContext context) : base(context)
        {
            _context = context;
        }
        // Include Category & Organizer for all events
        public override async Task<List<EEvent>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        public override async Task<EEvent> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Get events by category
        public async Task<IEnumerable<EEvent>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Where(e => e.CategoryId == categoryId)
                .ToListAsync();
        }

        // Get events by organizer
        public async Task<IEnumerable<EEvent>> GetByOrganizerAsync(int organizerId)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();
        }
    }
}
