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
                .ForMember(d => d.CategoryId,   o => o.MapFrom(s => s.Category.Id))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.Id,            o => o.Ignore())
                .ForMember(d => d.CreatedAtUtc,  o => o.Ignore())
                .ForMember(d => d.UpdatedAtUtc,  o => o.Ignore())
                .ForMember(d => d.Category,      o => o.MapFrom(s => new Category { Id = s.CategoryId }));

            CreateMap<UpdateProductDto, Product>()
                .ForMember(d => d.Id,            o => o.Ignore())
                .ForMember(d => d.CreatedAtUtc,  o => o.Ignore())
                .ForMember(d => d.UpdatedAtUtc,  o => o.Ignore())
                .ForMember(d => d.Category,      o => o.MapFrom(s => new Category { Id = s.CategoryId }));

            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products != null ? s.Products.Count : 0));

            CreateMap<CreateCategoryDto, Category>()
                .ForMember(d => d.Id,       o => o.Ignore())
                .ForMember(d => d.Products, o => o.Ignore());

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(d => d.Id,       o => o.Ignore())
                .ForMember(d => d.Products, o => o.Ignore());
        }
    }
}