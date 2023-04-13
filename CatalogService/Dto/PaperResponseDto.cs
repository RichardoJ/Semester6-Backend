namespace CatalogService.Dto
{
    //To handle the pagination
    public class PaperResponseDto
    {
        public List<PaperDto> paperDtos { get; set; } = new List<PaperDto>();

        public int Pages { get; set; }

        public int CurrentPages { get; set; }
    }
}
