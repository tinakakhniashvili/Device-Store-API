using DeviceStore.Dto;
using DeviceStore.Models;

namespace DeviceStore.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync(CancellationToken ct = default);
        Task<Product?> GetProductAsync(int productId, CancellationToken ct = default);
        Task<bool> ProductExistsAsync(int productId, CancellationToken ct = default);
        Task<Product> CreateProductAsync(Product product, CancellationToken ct = default);
        Task UpdateProductAsync(Product product, CancellationToken ct = default);
        Task DeleteProductAsync(Product product, CancellationToken ct = default);

        Task<(List<Product> Items, int TotalCount)> QueryProductsAsync(ProductQuery query, CancellationToken ct = default);
    }
}