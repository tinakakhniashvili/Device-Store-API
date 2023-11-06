using TinasAppleStore.Data;
using TinasAppleStore.Interfaces;
using TinasAppleStore.Models;

namespace TinasAppleStore.Repository
{
    public class ProductRepository : IProductRepository
    {
        private DataContext _context;
        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateProduct(Product product)
        {
            _context.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _context.Remove(product);
            return Save();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public bool ProductExists(int productId)
        {
            return _context.Products.Any(p => p.Id == productId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateProduct(Product product)
        {
            var foundProduct = GetProduct(product.Id);

            if (foundProduct != null)
            {
                foundProduct.Name = product.Name;
                foundProduct.Price = product.Price;
                foundProduct.Description = product.Description;

                return Save();
            }
            return false;
        }
    }
}
