using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeviceStore.Data;
using DeviceStore.Models;
using DeviceStore.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeviceStore.Test
{
    public class CategoryRepositoryTests
    {
        private static DataContext CreateContextWithSeed()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);

            context.Categories.AddRange(new[]
            {
                new Category { Id = 1, Name = "Electronics", Products = new List<Product>() },
                new Category { Id = 2, Name = "Furniture",   Products = new List<Product>() }
            });

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCategories()
        {
            using var context = CreateContextWithSeed();
            var repo = new CategoryRepository(context);

            var result = await repo.GetAllAsync(CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Electronics");
            Assert.Contains(result, c => c.Name == "Furniture");
        }

        [Fact]
        public async Task GetAsync_ReturnsCategory_WhenCategoryExists()
        {
            using var context = CreateContextWithSeed();
            var repo = new CategoryRepository(context);

            var result = await repo.GetAsync(1, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Electronics", result!.Name);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenCategoryDoesNotExist()
        {
            using var context = CreateContextWithSeed();
            var repo = new CategoryRepository(context);

            var result = await repo.GetAsync(999, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenCategoryExists()
        {
            using var context = CreateContextWithSeed();
            var repo = new CategoryRepository(context);

            var exists = await repo.ExistsAsync(2, CancellationToken.None);

            Assert.True(exists);
        }

        [Fact]
        public async Task Create_Update_Delete_Category_Works()
        {
            using var context = CreateContextWithSeed();
            var repo = new CategoryRepository(context);

            var created = await repo.CreateAsync(new Category { Name = "Appliances" }, CancellationToken.None);
            Assert.True(created.Id > 0);

            created.Name = "Home Appliances";
            await repo.UpdateAsync(created, CancellationToken.None);

            var fetched = await repo.GetByNameAsync("home appliances", CancellationToken.None);
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched!.Id);

            await repo.DeleteAsync(created, CancellationToken.None);
            var exists = await repo.ExistsAsync(created.Id, CancellationToken.None);
            Assert.False(exists);
        }
    }
}
