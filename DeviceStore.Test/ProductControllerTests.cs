using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceStore.Controllers;
using DeviceStore.Interfaces;
using DeviceStore.Models;
using DeviceStore.Dto;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using DeviceStore.Test.MockData;

namespace DeviceStore.Test
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public ProductControllerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ProductController(_productRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            var products = ProductMockData.GetSampleProducts();
            _productRepositoryMock.Setup(repo => repo.GetProductsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                       .Returns(products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price, CategoryName = p.Category.Name }).ToList());
            
            var result = await _controller.GetProducts(CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductDto>>(okResult.Value);
            returnValue.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WithProduct_WhenProductExists()
        {
            var product = ProductMockData.GetSampleProduct();
            _productRepositoryMock.Setup(repo => repo.GetProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                       .Returns(new ProductDto { Id = 1, Name = "Laptop", Price = 1000, CategoryName = "Electronics" });
            
            var result = await _controller.GetProduct(1, CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProductDto>(okResult.Value);
            returnValue.Name.Should().Be("Laptop");
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _productRepositoryMock.Setup(repo => repo.GetProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
            
            var result = await _controller.GetProduct(1, CancellationToken.None);
            
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
