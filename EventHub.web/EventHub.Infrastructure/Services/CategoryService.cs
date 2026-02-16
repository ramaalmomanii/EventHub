using AutoMapper;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Categories;
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
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly IMapper _mapper;


        public CategoryService(IGenericRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
        }

        public async Task<CategoryReadDto> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<CategoryReadDto> AddAsync(CategoryCreateDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            
            await _repository.AddAsync(category);
            return _mapper.Map<CategoryReadDto>(category);
        }

        public async Task<CategoryReadDto> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);

            return _mapper.Map<CategoryReadDto>(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
