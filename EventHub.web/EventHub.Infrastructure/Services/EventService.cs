using AutoMapper;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Events;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IGenericRepository<EEvent> _repository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Registration> _registrationRepository;



        public EventService(
       IRegistrationRepository registrationRepository,
       IEventRepository repository,
       IMapper mapper)
        {
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<EventReadDto>> GetAllAsync()
        {
            var events = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventReadDto>>(events);
        }

        public async Task<EventReadDto> GetByIdAsync(int id)
        {
            var ev = await _repository.GetByIdAsync(id);
            return _mapper.Map<EventReadDto>(ev);
        }

        public async Task<EventReadDto> AddAsync(EventCreateDto dto, int currentUserId)
        {
            var ev = _mapper.Map<EEvent>(dto);
            ev.OrganizerId = currentUserId;
            
            ev.CreatedAt = DateTime.UtcNow;
            ev.EndDate = DateTime.UtcNow.AddDays(7);
            ev.Capacity = 20;
            ev.AvailableSeats = dto.Capacity; // Initialize available seats to capacity
            ev.Status = "Active";
            await _repository.AddAsync(ev);
            return _mapper.Map<EventReadDto>(ev);
        }


        public async Task<EventReadDto> UpdateAsync(int id, EventUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);

            return _mapper.Map<EventReadDto>(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }


     
        public async Task<IEnumerable<EventReadDto>> GetEventsByUserAsync(int userId)
        {
            var registrations = await _registrationRepository.FindAsync(r => r.AttendeeId == userId);

            if (registrations == null || !registrations.Any())
                return new List<EventReadDto>();


            var eventIds = registrations.Select(r => r.EventId).ToList();

            var events = await _repository.FindAsync(e => eventIds.Contains(e.Id));

            return _mapper.Map<IEnumerable<EventReadDto>>(events);
        }

        public async Task RegisterUserToEventAsync(int eventId, int userId)
        {
            var ev = await _repository.GetByIdAsync(eventId);
            if (ev == null) throw new Exception("Event not found");

            var existing = await _registrationRepository.FindAsync(r => r.EventId == eventId && r.AttendeeId == userId);
            if (existing.Any()) throw new Exception("User already registered");

            var registration = new Registration { EventId = eventId, AttendeeId = userId, RegistrationDate = DateTime.UtcNow };
            await _registrationRepository.AddAsync(registration);
        }


        public async Task UnregisterUserFromEventAsync(int eventId, int userId)
        {
            var registration = (await _registrationRepository.FindAsync(r => r.EventId == eventId && r.AttendeeId == userId)).FirstOrDefault();
            if (registration == null) throw new Exception("Registration not found");

            await _registrationRepository.DeleteAsync(registration.Id);
        }
    }
}
