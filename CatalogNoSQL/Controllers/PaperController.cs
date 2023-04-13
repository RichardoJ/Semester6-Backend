using Amazon.Auth.AccessControlPolicy;
using CatalogNoSQL.Model;
using CatalogNoSQL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogNoSQL.Controllers
{
    [ApiController]
    [Route("api/paper")]
    public class PaperController : ControllerBase
    {
        private readonly IPaperService _paperService;

        public PaperController(IPaperService paperService)
        {
            _paperService = paperService;
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
        public async Task<IEnumerable<Paper>> Get() =>
        await _paperService.GetAllPapersAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Paper>> Get(string id)
        {
            var paper = await _paperService.GetPaperByIdAsync(id);

            if (paper is null)
            {
                return NotFound();
            }

            return paper;
        }

        [HttpGet("author/{id}")]
        public async Task<ActionResult<IEnumerable<Paper>>> GetByAuthor(int id)
        {
            var paper = await _paperService.GetAllPapersByAuthorAsync(id);

            if (paper is null)
            {
                return NotFound();
            }

            return paper.ToList();
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(Paper newPaper)
        //{
        //    await _paperService.CreatePaper(newPaper);

        //    return CreatedAtAction(nameof(Get), new { id = newPaper.Id }, newPaper);
        //}

        //[HttpPut("update/{id:length(24)}")]
        //public async Task<IActionResult> Update(string id, Paper updatedPaper)
        //{
        //    var paper = await _paperService.GetPaperById(id);

        //    if (paper is null)
        //    {
        //        return NotFound();
        //    }

        //    updatedPaper.Id = paper.Id;

        //    await _paperService.UpdatePaper(id, updatedPaper);

        //    return NoContent();
        //}

        //[HttpDelete("delete/{id:length(24)}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    var book = await _paperService.GetPaperById(id);

        //    if (book is null)
        //    {
        //        return NotFound();
        //    }

        //    await _paperService.DeletePaper(id);

        //    return NoContent();
        //}
    }
}
