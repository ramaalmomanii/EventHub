using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Interfaces.Services
{
    public interface IPageService : IGenericService<PageReadDto>
    {
        Task<PageReadDto?> GetBySlugAsync(string slug);
    }
}

