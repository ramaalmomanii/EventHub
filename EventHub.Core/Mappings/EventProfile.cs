using AutoMapper;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Events;
using EventHub.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EEvent, EventReadDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.FullName));

            CreateMap<EventCreateDto, EEvent>();
            CreateMap<EventUpdateDto, EEvent>();
        }
    }
}

