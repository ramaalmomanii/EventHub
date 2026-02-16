using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<List<T>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<T> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task AddAsync(T entity) => await _repository.AddAsync(entity);
        public async Task UpdateAsync(T entity) => await _repository.UpdateAsync(entity);
        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }

}
