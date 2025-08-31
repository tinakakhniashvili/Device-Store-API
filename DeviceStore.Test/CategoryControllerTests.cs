using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceStore.Controllers;
using DeviceStore.Dto;
using DeviceStore.Interfaces;
using DeviceStore.Models;
using DeviceStore.Test.MockData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DeviceStore.Test
{
    public class CategoryControllerTests
    {
        private readonly CategoryController _controller;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
        private readonly IMapper _mapper;

        public CategoryControllerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>()
                   .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products != null ? s.Products.Count : 0));
            });
            _mapper = mapperConfig.CreateMapper();
            _controller = new CategoryController(_categoryRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfCategories()
        {
            var categories = CategoryMockData.GetSampleCategories();
            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);

            var result = await _controller.GetAll(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CategoryDto>>(okResult.Value);
            returnValue.Should().HaveCount(categories.Count);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithCategory_WhenCategoryExists()
        {
            var category = CategoryMockData.GetSampleCategory();
            _categoryRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var result = await _controller.Get(1, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDto>(okResult.Value);
            returnValue.Id.Should().Be(category.Id);
            returnValue.Name.Should().Be(category.Name);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            _categoryRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

            var result = await _controller.Get(1, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
