using PublishNoSQL.Dto;
using PublishNoSQL.Model;

namespace PublishNoSQL.Profile
{
    public class PaperProfile : AutoMapper.Profile
    {
        public PaperProfile()
        {
            //CreateMap<PaperWriteDto, Paper>();
            CreateMap<Paper, PaperUpdatedDto>();
            CreateMap<Paper, PaperPublishedDto>();
            CreateMap<PaperPublishedDto, Paper>();
        }
    }
}
