using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Registertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Interfaces.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationReadDto> RegisterAsync(RegistrationCreateDto dto, int userId);
        Task CancelRegistrationAsync(int registrationId, int currentUserId, string role);
        Task<RegistrationReadDto?> GetByUserAndEventAsync(int userId, int eventId);
        Task<IEnumerable<RegistrationReadDto>> GetByEventAsync(int eventId);
        Task<IEnumerable<RegistrationReadDto>> GetRegistrationsByUserAsync(int userId);
    }
}

