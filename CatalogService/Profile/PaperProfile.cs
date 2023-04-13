using CatalogService.Dto;
using CatalogService.Model;

namespace CatalogService.Profile
{
    public class PaperProfile : AutoMapper.Profile
    {
        public PaperProfile()
        {
            CreateMap<Paper, PaperDto>();
            CreateMap<PaperDto, Paper>();
            CreateMap<PaperPublishedDto, Paper>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.category));
        }
    }
}
