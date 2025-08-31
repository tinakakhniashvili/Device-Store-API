using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceStore.Controllers;
using DeviceStore.Dto;
using DeviceStore.Helper;
using DeviceStore.Interfaces;
using DeviceStore.Test.MockData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DeviceStore.Test
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly Mock<IProductRepository> _repo = new();
        private readonly IMapper _mapper;

        public ProductControllerTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfiles>());
            _mapper = cfg.CreateMapper();
            _controller = new ProductController(_repo.Object, _mapper);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            var products = ProductMockData.GetSampleProducts();
            _repo.Setup(r => r.GetProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var result = await _controller.GetProducts(CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsType<List<ProductDto>>(ok.Value);
            dtos.Should().HaveCount(products.Count);

            for (int i = 0; i < products.Count; i++)
            {
                dtos[i].Id.Should().Be(products[i].Id);
                dtos[i].Name.Should().Be(products[i].Name);
                dtos[i].Price.Should().Be(products[i].Price);
                dtos[i].CategoryId.Should().Be(products[i].Category.Id);
                dtos[i].CategoryName.Should().Be(products[i].Category.Name);
            }
        }
    }
}