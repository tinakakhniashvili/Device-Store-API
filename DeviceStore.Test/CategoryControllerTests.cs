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
    public class CategoryControllerTests
    {
        private readonly CategoryController _controller;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public CategoryControllerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new CategoryController(_categoryRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfCategories()
        {
            var categories = CategoryMockData.GetSampleCategories();
            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);
            _mapperMock.Setup(m => m.Map<List<CategoryDto>>(It.IsAny<List<Category>>()))
                       .Returns(categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, ProductCount = 0 }).ToList());
            
            var result = await _controller.GetAll(CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CategoryDto>>(okResult.Value);
            returnValue.Should().HaveCount(2);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithCategory_WhenCategoryExists()
        {
            var category = CategoryMockData.GetSampleCategory();
            _categoryRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
                       .Returns(new CategoryDto { Id = 1, Name = "Electronics", ProductCount = 0 });
            
            var result = await _controller.Get(1, CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDto>(okResult.Value);
            returnValue.Name.Should().Be("Electronics");
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
