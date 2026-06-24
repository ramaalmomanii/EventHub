using EventHub.Core.DTOs.Events;

namespace EventHub.Core.Interfaces.Services
{
    public interface IEventSummaryService
    {
        Task<EventSummaryDto> GetSummaryAsync(int eventId, string provider);
    }
}
