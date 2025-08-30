using DeviceStore.Data;
using DeviceStore.Interfaces;
using DeviceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceStore.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        public CategoryRepository(DataContext context) => _context = context;

        public Task<List<Category>> GetAllAsync(CancellationToken ct = default) =>
            _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);

        public Task<Category?> GetAsync(int id, CancellationToken ct = default) =>
            _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task<Category?> GetByNameAsync(string name, CancellationToken ct = default) =>
            _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower().Trim(), ct);

        public async Task<Category> CreateAsync(Category category, CancellationToken ct = default)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(ct);
            return category;
        }

        public async Task UpdateAsync(Category category, CancellationToken ct = default)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Category category, CancellationToken ct = default)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            _context.Categories.AnyAsync(c => c.Id == id, ct);
    }
}