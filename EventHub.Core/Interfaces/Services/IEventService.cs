using EventHub.Core.DTOs.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EventHub.Core.Interfaces.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventReadDto>> GetAllAsync();
        Task<EventReadDto> GetByIdAsync(int id);
        Task<EventReadDto> AddAsync(EventCreateDto dto, int userId);
        Task<EventReadDto> UpdateAsync(int id, EventUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<EventReadDto>> GetEventsByUserAsync(int userId);
        Task RegisterUserToEventAsync(int eventId, int userId);
        Task UnregisterUserFromEventAsync(int eventId, int userId);


    }
}

