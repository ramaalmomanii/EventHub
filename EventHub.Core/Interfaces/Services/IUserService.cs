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
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(int id);
        Task<UserReadDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserReadDto>> GetByRoleAsync(string role);
        Task<UserReadDto> RegisterAsync(UserCreateDto dto);
        Task<UserReadDto> UpdateProfileAsync(int userId, UserUpdateDto dto);
        Task<TokenResponseDto?> LoginAsync(string email, string password);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(string token, string newPassword);
        Task<string> GenerateEmailVerificationTokenAsync(string email);
        Task VerifyEmailAsync(string token);
        Task<UserReadDto> CreateUserAsync(AdminCreateUserDto dto);
        Task<UserReadDto> UpdateUserAsync(int id, AdminUpdateUserDto dto);
        Task DeleteUserAsync(int id);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);

    }

}

