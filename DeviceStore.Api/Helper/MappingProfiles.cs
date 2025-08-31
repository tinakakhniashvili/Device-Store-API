using AutoMapper;
using DeviceStore.Dto;
using DeviceStore.Models;

namespace DeviceStore.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            
            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count));
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}