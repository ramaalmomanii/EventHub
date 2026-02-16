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
        Task<IEnumerable<RegistrationReadDto>> GetRegistrationsByUserAsync(int userId);

        Task<RegistrationReadDto> RegisterAsync(RegistrationCreateDto dto,int userid);
        Task<bool> CancelRegistrationAsync(int registrationId);
        Task<RegistrationReadDto?> GetByUserAndEventAsync(int userId, int eventId);
        Task<List<RegistrationReadDto>> GetByEventAsync(int eventId);
    }
}

