using System.Linq.Expressions;
using DeviceStore.Data;
using DeviceStore.Dto;
using DeviceStore.Interfaces;
using DeviceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceStore.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context) => _context = context;

        public async Task<List<Product>> GetProductsAsync(CancellationToken ct = default)
            => await _context.Products.AsNoTracking().ToListAsync(ct);

        public async Task<Product?> GetProductAsync(int id, CancellationToken ct = default)
            => await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<bool> ProductExistsAsync(int productId, CancellationToken ct = default)
            => await _context.Products.AnyAsync(p => p.Id == productId, ct);

        public async Task<Product> CreateProductAsync(Product product, CancellationToken ct = default)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(ct);
            return product;
        }

        public async Task UpdateProductAsync(Product product, CancellationToken ct = default)
        {
            product.UpdatedAtUtc = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteProductAsync(Product product, CancellationToken ct = default)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
        }
        
        // inside ProductRepository
        public async Task<(List<Product> Items, int TotalCount)> QueryProductsAsync(ProductQuery query, CancellationToken ct = default)
        {
            IQueryable<Product> q = _context.Products
                .Include(p => p.Category)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(p =>
                    EF.Functions.Like(p.Name, $"%{s}%") ||
                    (p.Description != null && EF.Functions.Like(p.Description, $"%{s}%"))
                );
            }
            
            if (query.CategoryId.HasValue)
                q = q.Where(p => p.CategoryId == query.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                var c = query.Category.Trim().ToLower();
                q = q.Where(p => p.Category.Name.ToLower() == c);
            }

            q = ApplySort(q, query.Sort);

            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return (items, total);
        }
        private static IQueryable<Product> ApplySort(IQueryable<Product> q, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return q.OrderBy(p => p.Name);

            IOrderedQueryable<Product>? ordered = null;
            foreach (var token in sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var desc = token.StartsWith("-");
                var field = desc ? token[1..] : token;

                Expression<Func<Product, object>> key = field.ToLower() switch
                {
                    "name" => p => p.Name,
                    "price" => p => p.Price,
                    "createdatutc" => p => p.CreatedAtUtc,
                    _ => p => p.Name 
                };

                if (ordered is null)
                    ordered = desc ? q.OrderByDescending(key) : q.OrderBy(key);
                else
                    ordered = desc ? ordered.ThenByDescending(key) : ordered.ThenBy(key);
            }

            return ordered ?? q.OrderBy(p => p.Name);
        }
    }
}