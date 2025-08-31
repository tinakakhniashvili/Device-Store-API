using System.Collections.Generic;
using DeviceStore.Models;
using DeviceStore.Dto;

namespace DeviceStore.Test.MockData
{
    public static class ProductMockData
    {
        public static Product GetSampleProduct() =>
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Electronics" }
            };

        public static ProductDto GetSampleProductDto() =>
            new ProductDto
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000,
                CategoryId = 1,
                CategoryName = "Electronics"
            };

        public static List<Product> GetSampleProducts() =>
            new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Price = 1000,
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electronics" }
                },
                new Product
                {
                    Id = 2,
                    Name = "Phone",
                    Price = 500,
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Electronics" }
                }
            };
    }
}
