using PublishService.Dto;
using PublishService.Model;

namespace PublishService.Profile
{
    public class PaperProfile : AutoMapper.Profile
    {
        public PaperProfile()
        {
            CreateMap<Paper, PaperDto>();
            CreateMap<PaperWriteDto, Paper>();
            CreateMap<PaperUpdateDto, PaperUpdatedDto>();
            CreateMap<PaperWriteDto, PaperPublishedDto>()
                .ForMember(dest => dest.category, opt => opt.MapFrom(src => src.category.Id));
        }
    }
}
