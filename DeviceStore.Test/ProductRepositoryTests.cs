using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceStore.Data;
using DeviceStore.Dto;
using DeviceStore.Models;
using DeviceStore.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeviceStore.Test
{
    public class ProductRepositoryTests
    {
        private static DataContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .EnableSensitiveDataLogging()
                .Options;

            var ctx = new DataContext(options);

            
            ctx.Database.EnsureCreated();
            return ctx;
        }

        private static async Task SeedAsync(DataContext ctx)
        {
            var catTools = new Category { Id = 1, Name = "Tools" };
            var catPhones = new Category { Id = 2, Name = "Phones" };

            var now = DateTime.UtcNow.AddDays(-2);
            var items = new List<Product>
            {
                new Product { Id = 1, Name = "A-Clamp", Description = "Metal clamp", Price = 9.99, CategoryId = 1, Category = catTools, CreatedAtUtc = now.AddHours(1) },
                new Product { Id = 2, Name = "Pro Drill", Description = "Pro-grade drill", Price = 149.00, CategoryId = 1, Category = catTools, CreatedAtUtc = now.AddHours(2) },
                new Product { Id = 3, Name = "Basic Saw", Description = "Saw tool", Price = 29.50, CategoryId = 1, Category = catTools, CreatedAtUtc = now.AddHours(3) },
                new Product { Id = 4, Name = "Zen Phone", Description = "Android device", Price = 399.00, CategoryId = 2, Category = catPhones, CreatedAtUtc = now.AddHours(4) },
                new Product { Id = 5, Name = "Pro Saw", Description = "Professional saw", Price = 89.00, CategoryId = 1, Category = catTools, CreatedAtUtc = now.AddHours(5) }
            };

            await ctx.AddRangeAsync(catTools, catPhones);
            await ctx.AddRangeAsync(items);
            await ctx.SaveChangesAsync();
        }

        private static ProductRepository CreateRepo(DataContext ctx) => new ProductRepository(ctx);

        [Fact]
        public async Task GetProductsAsync_returns_all_products()
        {
            var ctx = CreateContext(nameof(GetProductsAsync_returns_all_products));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var items = await repo.GetProductsAsync();

            items.Should().HaveCount(5);
            items.Select(p => p.Name).Should().Contain(new[] { "A-Clamp", "Pro Drill", "Basic Saw", "Zen Phone", "Pro Saw" });
        }

        [Fact]
        public async Task GetProductAsync_returns_single_product_by_id_or_null()
        {
            var ctx = CreateContext(nameof(GetProductAsync_returns_single_product_by_id_or_null));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);

            var p2 = await repo.GetProductAsync(2);
            p2.Should().NotBeNull();
            p2!.Name.Should().Be("Pro Drill");

            var p999 = await repo.GetProductAsync(999);
            p999.Should().BeNull();
        }

        [Fact]
        public async Task ProductExistsAsync_returns_true_when_exists_false_otherwise()
        {
            var ctx = CreateContext(nameof(ProductExistsAsync_returns_true_when_exists_false_otherwise));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);

            (await repo.ProductExistsAsync(3)).Should().BeTrue();
            (await repo.ProductExistsAsync(321)).Should().BeFalse();
        }

        [Fact]
        public async Task CreateProductAsync_persists_and_returns_entity()
        {
            var ctx = CreateContext(nameof(CreateProductAsync_persists_and_returns_entity));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var product = new Product
            {
                Name = "Mini Grinder",
                Description = "Compact grinder",
                Price = 45.75,
                CategoryId = 1,
                CreatedAtUtc = DateTime.UtcNow
            };

            var created = await repo.CreateProductAsync(product);

            created.Id.Should().NotBe(0);
            (await ctx.Products.CountAsync()).Should().Be(6);
        }

        [Fact]
        public async Task UpdateProductAsync_updates_fields_and_sets_UpdatedAtUtc()
        {
            var ctx = CreateContext(nameof(UpdateProductAsync_updates_fields_and_sets_UpdatedAtUtc));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var item = await repo.GetProductAsync(1);
            item!.Price = 12.34;

            var before = DateTime.UtcNow;
            await repo.UpdateProductAsync(item);

            var updated = await repo.GetProductAsync(1);
            updated!.Price.Should().Be(12.34);
            updated.UpdatedAtUtc.Should().NotBeNull();
            updated.UpdatedAtUtc!.Value.Should().BeOnOrAfter(before.AddSeconds(-1)); 
        }

        [Fact]
        public async Task DeleteProductAsync_removes_entity()
        {
            var ctx = CreateContext(nameof(DeleteProductAsync_removes_entity));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var item = await repo.GetProductAsync(4);
            await repo.DeleteProductAsync(item!);

            (await ctx.Products.AnyAsync(p => p.Id == 4)).Should().BeFalse();
        }

        [Fact]
        public async Task QueryProductsAsync_filters_by_search()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_filters_by_search));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var query = new ProductQuery
            {
                Search = "Pro", 
                Page = 1,
                PageSize = 20
            };

            var (items, total) = await repo.QueryProductsAsync(query);
            total.Should().Be(2);
            items.Select(i => i.Name).Should().BeEquivalentTo(new[] { "Pro Drill", "Pro Saw" });
        }

        [Fact]
        public async Task QueryProductsAsync_filters_by_category_id()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_filters_by_category_id));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var query = new ProductQuery
            {
                CategoryId = 2, 
                Page = 1,
                PageSize = 10
            };

            var (items, total) = await repo.QueryProductsAsync(query);
            total.Should().Be(1);
            items.Single().Name.Should().Be("Zen Phone");
        }

        [Fact]
        public async Task QueryProductsAsync_filters_by_category_name_case_insensitive()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_filters_by_category_name_case_insensitive));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var query = new ProductQuery
            {
                Category = "tools",
                Page = 1,
                PageSize = 20
            };

            var (items, total) = await repo.QueryProductsAsync(query);
            total.Should().Be(4);
            items.Should().OnlyContain(p => p.CategoryId == 1);
        }

        [Fact]
        public async Task QueryProductsAsync_applies_default_sort_by_name()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_applies_default_sort_by_name));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var query = new ProductQuery { Page = 1, PageSize = 10 };

            var (items, total) = await repo.QueryProductsAsync(query);
            total.Should().Be(5);

            items.Select(p => p.Name).Should().ContainInOrder("A-Clamp", "Basic Saw", "Pro Drill", "Pro Saw", "Zen Phone");
        }

        [Fact]
        public async Task QueryProductsAsync_applies_sort_single_and_multi_field()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_applies_sort_single_and_multi_field));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            
            var q1 = new ProductQuery { Sort = "-price", Page = 1, PageSize = 10 };
            var (items1, _) = await repo.QueryProductsAsync(q1);
            items1.Select(p => p.Price).Should().BeInDescendingOrder();

            
            var q2 = new ProductQuery { Sort = "price,-name", Page = 1, PageSize = 10 };
            var (items2, _) = await repo.QueryProductsAsync(q2);
            items2.Select(p => p.Price).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task QueryProductsAsync_supports_sort_by_createdatutc_token()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_supports_sort_by_createdatutc_token));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);

            var q = new ProductQuery { Sort = "-createdatutc", Page = 1, PageSize = 10 };
            var (items, _) = await repo.QueryProductsAsync(q);

            
            items.First().Name.Should().Be("Pro Saw"); 
        }

        [Fact]
        public async Task QueryProductsAsync_applies_paging_after_filters_and_sort()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_applies_paging_after_filters_and_sort));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var query = new ProductQuery
            {
                Page = 2,
                PageSize = 2,
                Sort = "name" 
            };

            var (items, total) = await repo.QueryProductsAsync(query);
            total.Should().Be(5);
            items.Should().HaveCount(2);
            items.Select(p => p.Name).Should().ContainInOrder("Pro Drill", "Pro Saw"); 
        }

        [Fact]
        public async Task QueryProductsAsync_unknown_sort_field_falls_back_to_name()
        {
            var ctx = CreateContext(nameof(QueryProductsAsync_unknown_sort_field_falls_back_to_name));
            await SeedAsync(ctx);

            var repo = CreateRepo(ctx);
            var query = new ProductQuery
            {
                Sort = "doesNotExist",
                Page = 1,
                PageSize = 10
            };

            var (items, _) = await repo.QueryProductsAsync(query);
            items.Select(p => p.Name).Should().ContainInOrder("A-Clamp", "Basic Saw", "Pro Drill", "Pro Saw", "Zen Phone");
        }
    }
}
