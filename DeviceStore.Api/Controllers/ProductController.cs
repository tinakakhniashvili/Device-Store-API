using AutoMapper;
using DeviceStore.Dto;
using DeviceStore.Interfaces;
using DeviceStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        public async Task<IActionResult> GetProducts(CancellationToken ct)
        {
            var products = await _repo.GetProductsAsync(ct);
            var dto = _mapper.Map<List<ProductDto>>(products);
            return Ok(dto);
        }

        [HttpGet("{productId:int}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct(int productId, CancellationToken ct)
        {
            var product = await _repo.GetProductAsync(productId, ct);
            if (product is null) return NotFound();
            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto body, CancellationToken ct)
        {
            var existsByName = (await _repo.GetProductsAsync(ct))
                .Any(p => p.Name.Trim().Equals(body.Name.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existsByName) return Conflict(new { message = "Product with this name already exists." });

            var entity = _mapper.Map<Product>(body);
            entity = await _repo.CreateProductAsync(entity, ct);

            var dto = _mapper.Map<ProductDto>(entity);
            return CreatedAtAction(nameof(GetProduct), new { productId = dto.Id }, dto);
        }

        [HttpPut("{productId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProductDto body, CancellationToken ct)
        {
            var entity = await _repo.GetProductAsync(productId, ct);
            if (entity is null) return NotFound();
            
            _mapper.Map(body, entity);
            await _repo.UpdateProductAsync(entity, ct);
            return NoContent();
        }

        [HttpDelete("{productId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int productId, CancellationToken ct)
        {
            var entity = await _repo.GetProductAsync(productId, ct);
            if (entity is null) return NotFound();

            await _repo.DeleteProductAsync(entity, ct);
            return NoContent();
        }
    }
}
