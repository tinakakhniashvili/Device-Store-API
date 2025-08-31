using Moq;
using DeviceStore.Models;
using DeviceStore.Repository;
using DeviceStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeviceStore.Test
{
    public class CategoryRepositoryTests
    {
        private readonly CategoryRepository _repository;
        private readonly Mock<DataContext> _mockContext;
        private readonly Mock<DbSet<Category>> _mockCategoryDbSet;

        public CategoryRepositoryTests()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Products = new List<Product>() },
                new Category { Id = 2, Name = "Furniture", Products = new List<Product>() }
            }.AsQueryable();

            _mockCategoryDbSet = new Mock<DbSet<Category>>();

            _mockCategoryDbSet.As<IQueryable<Category>>()
                .Setup(m => m.Provider).Returns(categories.Provider);
            _mockCategoryDbSet.As<IQueryable<Category>>()
                .Setup(m => m.Expression).Returns(categories.Expression);
            _mockCategoryDbSet.As<IQueryable<Category>>()
                .Setup(m => m.ElementType).Returns(categories.ElementType);
            _mockCategoryDbSet.As<IQueryable<Category>>()
                .Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());
            
            _mockContext = new Mock<DataContext>();
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            
            _repository = new CategoryRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCategories()
        {
            var result = await _repository.GetAllAsync(CancellationToken.None);
            
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Electronics");
            Assert.Contains(result, c => c.Name == "Furniture");
        }

        [Fact]
        public async Task GetAsync_ReturnsCategory_WhenCategoryExists()
        {
            var result = await _repository.GetAsync(1, CancellationToken.None);
            
            Assert.NotNull(result);
            Assert.Equal("Electronics", result?.Name);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenCategoryDoesNotExist()
        {
            var result = await _repository.GetAsync(3, CancellationToken.None);
            
            Assert.Null(result);
        }
        
    }
}
