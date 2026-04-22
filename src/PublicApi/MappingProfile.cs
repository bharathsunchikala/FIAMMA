using AutoMapper;
using Fiamma.ApplicationCore.Entities;
using Fiamma.PublicApi.CatalogBrandEndpoints;
using Fiamma.PublicApi.CatalogItemEndpoints;
using Fiamma.PublicApi.CatalogTypeEndpoints;

namespace Fiamma.PublicApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CatalogItem, CatalogItemDto>()
            .ForMember(dto => dto.Options, options => options.MapFrom(src => src.Options));
        CreateMap<ProductOption, ProductOptionDto>();
        CreateMap<CatalogType, CatalogTypeDto>()
            .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Type));
        CreateMap<CatalogBrand, CatalogBrandDto>()
            .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Brand));
    }
}

