using AutoMapper;
using TinasAppleStore.Models;
using TinasAppleStore.Dto;

namespace TinasAppleStore.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
