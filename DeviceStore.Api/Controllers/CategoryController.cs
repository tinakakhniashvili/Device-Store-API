using AutoMapper;
using DeviceStore.Dto;
using DeviceStore.Interfaces;
using DeviceStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var cats = await _repo.GetAllAsync(ct);
            return Ok(_mapper.Map<List<CategoryDto>>(cats));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var cat = await _repo.GetAsync(id, ct);
            if (cat is null) return NotFound();
            return Ok(_mapper.Map<CategoryDto>(cat));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), 201)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto body, CancellationToken ct)
        {
            var existing = await _repo.GetByNameAsync(body.Name, ct);
            if (existing is not null) return Conflict(new { message = "Category already exists." });

            var entity = await _repo.CreateAsync(_mapper.Map<Category>(body), ct);
            var dto = _mapper.Map<CategoryDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto body, CancellationToken ct)
        {
            var entity = await _repo.GetAsync(id, ct);
            if (entity is null) return NotFound();

            _mapper.Map(body, entity);
            await _repo.UpdateAsync(entity, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var entity = await _repo.GetAsync(id, ct);
            if (entity is null) return NotFound();
            
            await _repo.DeleteAsync(entity, ct);
            return NoContent();
        }
    }
}
