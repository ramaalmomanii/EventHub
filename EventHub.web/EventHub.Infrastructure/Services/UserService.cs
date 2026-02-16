using AutoMapper;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Users;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using EventHub.Infrastructure.Helpers;
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
using Microsoft.EntityFrameworkCore;



namespace EventHub.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtTokenGenerator _jwtTokenGenerator; 



        public UserService(IUserRepository userRepository, IMapper mapper, JwtTokenGenerator jwtTokenGenerator) 
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator; 

        }

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }


        public async Task<UserReadDto> RegisterAsync(UserCreateDto dto)
        {
            var users = await _userRepository.GetAllAsync();
            var existingUser = users.FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                throw new ApplicationException("البريد الإلكتروني مستخدم بالفعل");
            }
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = "Attendee";
            user.Status = "Active";
            user.FullName = dto.FullName;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.AddAsync(user);
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;


            return _jwtTokenGenerator.GenerateToken(user);

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


        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new ApplicationException("المستخدم غير موجود");

            // نولد كود مؤقت (token)
            var resetToken = Guid.NewGuid().ToString();

            // احفظه بقاعدة البيانات (كود بسيط، ممكن تعمل جدول PasswordResetTokens)
            user.ResetToken = resetToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateAsync(user);

            // ابعث ايميل للمستخدم
            // لاحقًا بنربط مع EmailSender
        }

        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepository.GetByResetTokenAsync(token);
            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
                throw new ApplicationException("الرابط غير صالح أو منتهي");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            await _userRepository.UpdateAsync(user);
        }



        public async Task<string> GenerateEmailVerificationTokenAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new ApplicationException("المستخدم غير موجود");

            var token = Guid.NewGuid().ToString();
            user.VerificationToken = token;
            user.VerifiedAt = null;
            await _userRepository.UpdateAsync(user);

            return token;
        }

        public async Task VerifyEmailAsync(string token)
        {
            var user = await _userRepository.GetByVerificationTokenAsync(token);
            if (user == null)
                throw new ApplicationException("الرابط غير صالح");

            user.VerifiedAt = DateTime.UtcNow;
            user.VerificationToken = null;
            await _userRepository.UpdateAsync(user);
        }


    }

}
