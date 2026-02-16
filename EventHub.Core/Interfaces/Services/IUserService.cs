using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.DTOs.Users;
using EventHub.Core.Entities;
using EventHub.Core.DTOs;

namespace EventHub.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserReadDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserReadDto>> GetByRoleAsync(string role);
        Task<UserReadDto> RegisterAsync(UserCreateDto dto);
        Task<TokenResponseDto?> LoginAsync(string email, string password);
        Task<IEnumerable<UserReadDto>> GetAllAsync();



        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(string token, string newPassword);
        Task VerifyEmailAsync(string token);
        Task<string> GenerateEmailVerificationTokenAsync(string email);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    }

}

