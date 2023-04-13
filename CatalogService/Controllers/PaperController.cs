using AutoMapper;
using CatalogService.Dto;
using CatalogService.Model;
using CatalogService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers
{
    [Route("api/paper")]
    [ApiController]
    public class PaperController : ControllerBase
    {

        private readonly IPaperService _service;
        private readonly IMapper _mapper;

        public PaperController(IPaperService paperService, IMapper mapper)
        {
            _service = paperService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Policy = "EmployeeOnly")]
        public ActionResult<String> TestAdmin()
        {
            Console.WriteLine("Testing Authentication and Authorization ...");
            return Ok("Hello Admin");
        }

        [HttpGet]
        [Route("reader")]
        [Authorize(Policy = "Public")]
        public ActionResult<String> TestReader()
        {
            Console.WriteLine("Testing Authentication and Authorization ...");
            return Ok("Hello Reader");
        }

        [HttpGet]
        [Route("public")]
        public ActionResult<String> TestController()
        {
            Console.WriteLine("Testing Authentication and Authorization ...");
            return Ok("Hello PUBLICCC");
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<PaperDto>> GetAllPaper()
        {
            var papers= _service.GetAllPapers();
            return Ok(_mapper.Map<IEnumerable<PaperDto>>(papers));
        }

        [HttpGet]
        [Route("pagination")]
        public async Task<ActionResult<List<PaperDto>>> GetAllPaperPagination([FromQuery] Parameters parameters)
        {
            var papers = _service.GetAllPapersPagination(parameters);

            return Ok(_mapper.Map<IEnumerable<PaperDto>>(papers));
        }

        [HttpGet("{id}")]
        public ActionResult<PaperDto> GetPaper(int id)
        {
            Console.WriteLine("Getting paper by id ...");

            var onePaper = _service.GetPaperById(id);
            if (onePaper == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<PaperDto>(onePaper));
            }
        }

        [HttpGet("author/{id}")]
        public ActionResult<PaperDto> GetPapersByAuthor(int id)
        {
            Console.WriteLine("Getting paper by authorid ...");

            var papers = _service.GetAllPapersByAuthor(id);
            if (papers == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<PaperDto>>(papers));
            }
        }
    }
}
