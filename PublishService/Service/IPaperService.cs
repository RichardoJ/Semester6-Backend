using PublishService.Model;

namespace PublishService.Service
{
    public interface IPaperService
    {
        Boolean AddPaper(Paper paper);

        Boolean UpdatePaper(Paper paper);

        Boolean RemovePaper(int id);
    }
}
