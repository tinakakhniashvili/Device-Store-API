using DeviceStore.Models;

namespace DeviceStore.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(CancellationToken ct = default);
        Task<Category?> GetAsync(int id, CancellationToken ct = default);
        Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);
        Task<Category> CreateAsync(Category category, CancellationToken ct = default);
        Task UpdateAsync(Category category, CancellationToken ct = default);
        Task DeleteAsync(Category category, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}