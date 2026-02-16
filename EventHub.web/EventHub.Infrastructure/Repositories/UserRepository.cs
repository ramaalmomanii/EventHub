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
    public class UserRepository:GenericRepository<User>,IUserRepository
    {
        private readonly EventHubDbContext _context;
        public UserRepository(EventHubDbContext context):base(context)
        {
            _context = context;
        }
        /*Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
   */
        // get user by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        // get users by role
        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            return await _context.Users.Where(u => u.Role == role).ToListAsync();
        }
        // authenticate user by email and password
        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        }

        // get user by reset token

        public async Task<User?> GetByResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == token);
        }

        public async Task<User?> GetByVerificationTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
        }

        // send email
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Use an email service like SendGrid, SMTP, etc. to send the email.
           
            await Task.CompletedTask;
        }






    }











}
