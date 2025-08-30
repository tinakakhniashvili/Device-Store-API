using DeviceStore.Data;
using DeviceStore.Models;

namespace DeviceStore;

public sealed class Seed
{
    private readonly DataContext _context;

    public Seed(DataContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    public void SeedDataContext()
    {
        if (_context.Categories.Any() || _context.Products.Any())
            return;
        
        var phone = new Category { Name = "Phones", Description = "Smartphones and mobile devices" };
        var laptop = new Category { Name = "Laptops", Description = "Notebooks and ultrabooks" };
        var audio = new Category { Name = "Audio", Description = "Headphones and speakers" };
        
        _context.Categories.AddRange(phone, laptop, audio);
        _context.SaveChanges(); 
        
        var products = new List<Product>
        {
            new() { Name = "iPhone 15", Description = "128GB", Price = 999, CategoryId = phone.Id },
            new() { Name = "AirPods Pro (2nd Gen)", Description = "ANC earbuds", Price = 249, CategoryId = audio.Id },
            new() { Name = "MacBook Air 13\"", Description = "M2, 8-core GPU", Price = 1199, CategoryId = laptop.Id }
        };
        
        _context.Products.AddRange(products);
        _context.SaveChanges(); 
    }
}