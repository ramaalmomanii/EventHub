using EventHub.Core.Constants;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Categories;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = $"{Permissions.Admin}")]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{Permissions.Admin}")]
        public async Task<ActionResult<CategoryReadDto>> GetById(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        
        [HttpPost]
        [Authorize(Roles = $"{Permissions.Admin}")]
        public async Task<ActionResult<CategoryReadDto>> Create([FromBody] CategoryCreateDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Permissions.Admin}")]
        public async Task<ActionResult<CategoryReadDto>> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Permissions.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
