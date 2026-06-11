using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.DTOs.Ticket;

namespace EventHub.Core.Interfaces.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketReadDto>> GetAllAsync();
        Task<TicketReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<TicketReadDto>> GetByUserIdAsync(int userId);
        Task<TicketReadDto> CreateAsync(TicketCreateDto dto);
        Task<TicketReadDto> UpdateAsync(int id, TicketUpdateDto dto);
        Task DeleteAsync(int id);
    }
}

