using AutoMapper;
using EventHub.Core.Constants;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Users;
using EventHub.Core.Entities;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using EventHub.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace EventHub.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            JwtTokenGenerator jwtTokenGenerator,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid user ID");

            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : _mapper.Map<UserReadDto>(user);
        }

        public async Task<IEnumerable<UserReadDto>> GetByRoleAsync(string role)
        {
            var users = await _userRepository.GetByRoleAsync(role);
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto> RegisterAsync(UserCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException("User data is required");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ValidationException("Full name is required");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password is required");

            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new ConflictException("Email already exists");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = Permissions.Attendee;
            user.Status = "Active";
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user);
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> UpdateProfileAsync(int userId, UserUpdateDto dto)
        {
            if (dto == null)
                throw new ValidationException("Update data is required");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> CreateUserAsync(AdminCreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required");

            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new ConflictException("Email already exists");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> UpdateUserAsync(int id, AdminUpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");

            user.FullName = dto.FullName;
            user.Role = dto.Role;
            user.Status = dto.Status;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");

            await _userRepository.DeleteAsync(id);
        }

        public async Task<TokenResponseDto?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Email and password are required");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenExpires = _jwtTokenGenerator.GetRefreshTokenExpiry();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpires = refreshTokenExpires;
            await _userRepository.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = refreshTokenExpires
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ValidationException("Refresh token is required");

            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpires < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token is invalid or expired");

            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenExpires = _jwtTokenGenerator.GetRefreshTokenExpiry();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpires = refreshTokenExpires;
            await _userRepository.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = refreshTokenExpires
            };
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is required");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new NotFoundException("User not found");

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendAsync(
                user.Email,
                "Password Reset",
                $"Your reset token: {user.ResetToken}"
            );
        }

        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ValidationException("Token is required");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ValidationException("New password is required");

            var user = await _userRepository.GetByResetTokenAsync(token);
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired token");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<string> GenerateEmailVerificationTokenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is required");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new NotFoundException("User not found");

            var token = Guid.NewGuid().ToString();
            user.VerificationToken = token;
            await _userRepository.UpdateAsync(user);

            await _emailService.SendAsync(
                user.Email,
                "Verify Your Email",
                $"Your verification token: {token}"
            );

            return token;
        }

        public async Task VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ValidationException("Token is required");

            var user = await _userRepository.GetByVerificationTokenAsync(token);
            if (user == null)
                throw new UnauthorizedException("Invalid verification token");

            user.VerifiedAt = DateTime.UtcNow;
            user.VerificationToken = null;
            await _userRepository.UpdateAsync(user);
        }
    }

}
