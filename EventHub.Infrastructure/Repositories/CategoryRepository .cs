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
    public class CategoryRepository:GenericRepository<Category>, ICategoryRepository
    {
        private readonly EventHubDbContext _context;
        public CategoryRepository(EventHubDbContext context) : base(context)
        {
            _context = context;
        }
        //get all categories with their related events as a collection
        public async Task<Category?> GetCategoryWithEventsAsync(int categoryId)
        {
            return await _context.Categories
              .Include(c => c.Events)
              .FirstOrDefaultAsync(c => c.Id == categoryId);

        }

    }
}
