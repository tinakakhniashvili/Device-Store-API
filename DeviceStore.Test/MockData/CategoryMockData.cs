using System.Collections.Generic;
using DeviceStore.Models;
using DeviceStore.Dto;

namespace DeviceStore.Test.MockData
{
    public static class CategoryMockData
    {
        public static Category GetSampleCategory() =>
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices",
                Products = new List<Product>()
            };

        public static CategoryDto GetSampleCategoryDto() =>
            new CategoryDto
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices",
                ProductCount = 0
            };

        public static List<Category> GetSampleCategories() =>
            new List<Category>
            {
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Furniture" }
            };
    }
}