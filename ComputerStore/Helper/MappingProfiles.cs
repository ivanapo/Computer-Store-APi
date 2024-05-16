using AutoMapper;
using ComputerStore.DTO;
using ComputerStore.Models;

namespace ComputerStore.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<ProductDTO, UpdateProductDTO>();
            CreateMap<UpdateProductDTO, ProductDTO>();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Discription, opt => opt.MapFrom(src => src.Discription))
                .ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories.Select(obj => obj.Name).ToList()));

            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Discription, opt => opt.MapFrom(src => src.Discription));

        }
    }
}

