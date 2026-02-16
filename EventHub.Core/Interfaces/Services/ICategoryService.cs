using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.DTOs.Categories;

using EventHub.Core.DTOs;

namespace EventHub.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDto>> GetAllAsync();
        Task<CategoryReadDto> GetByIdAsync(int id);
        Task<CategoryReadDto> AddAsync(CategoryCreateDto dto);
        Task<CategoryReadDto> UpdateAsync(int id, CategoryUpdateDto dto);
        Task DeleteAsync(int id);
    }
}

