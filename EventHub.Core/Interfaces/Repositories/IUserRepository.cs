using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;

namespace EventHub.Core.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<User?> AuthenticateAsync(string email, string passwordHash);



        Task<User?> GetByResetTokenAsync(string token);
        Task<User?> GetByVerificationTokenAsync(string token);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task SendEmailAsync(string to, string subject, string body);



    }
}

