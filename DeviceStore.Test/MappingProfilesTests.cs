using System.Collections.Generic;
using AutoMapper;
using DeviceStore.Dto;
using DeviceStore.Helper;
using DeviceStore.Models;
using Xunit;

namespace DeviceStore.Test
{
    public class MappingProfilesTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfiles>();
            });

            return config.CreateMapper();
        }

        [Fact]
        public void MappingConfiguration_IsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
            
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void Product_Maps_To_ProductDto_With_CategoryName()
        {
            var mapper = CreateMapper();
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Category = new Category { Name = "Electronics" }
            };
            
            var dto = mapper.Map<ProductDto>(product);
            
            Assert.Equal("Electronics", dto.CategoryName);
        }

        [Fact]
        public void Category_Maps_To_CategoryDto_With_ProductCount()
        {
            var mapper = CreateMapper();
            var category = new Category
            {
                Name = "Peripherals",
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Mouse" },
                    new Product { Id = 2, Name = "Keyboard" },
                    new Product { Id = 3, Name = "Headset" }
                }
            };
            
            var dto = mapper.Map<CategoryDto>(category);
            
            Assert.Equal(3, dto.ProductCount);
        }
    }
}