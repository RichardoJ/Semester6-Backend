using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublishNoSQL.Dto;
using PublishNoSQL.Model;
using PublishNoSQL.RabbitMQ;
using PublishNoSQL.Service;
using System.Linq;

namespace PublishNoSQL.Controllers
{
    [ApiController]
    [Route("api/publish")]
    public class PaperController : ControllerBase
    {
        private readonly IPaperService _paperService;
        private readonly IAzureStorage _storage;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public PaperController(IPaperService paperService, IMapper mapper, IMessageBusClient messageBusClient, IAzureStorage azureStorage)
        {
            _paperService = paperService;
            _storage = azureStorage;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
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

        [HttpPost]
        public async Task<ActionResult<Paper>> AddPaper(Paper paper)
        {
            Console.WriteLine("Adding Paper ... ");
            await _paperService.AddPaperAsync(paper);
            Console.WriteLine(paper.Id);

            var publishPaper = _mapper.Map<PaperPublishedDto>(paper);
            publishPaper.Event = "Publish";

            //Send Async Message
            try
            {
                _messageBusClient.PublishNewPaper(publishPaper);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send message async: {ex.Message}");
            }

            return Ok(paper);
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Policy = "Public")]
        public async Task<IActionResult> UpdatePaper(Paper paper)
        {
            Console.WriteLine("Updating Paper ...");

            var response = await _paperService.UpdatePaperAsync(paper);

            if(response == true)
            {
                var publishUpdatePaper = _mapper.Map<PaperUpdatedDto>(paper);
                publishUpdatePaper.Event = "Update_paper";

                //Send Async Message
                try
                {
                    _messageBusClient.UpdatePaper(publishUpdatePaper);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not send message async: {ex.Message}");
                }

                return Ok(paper);
            }
            else
            {
                return NotFound();
            }

            
        }

        [HttpDelete("delete/{id:length(24)}")]
        [Authorize(Policy = "Public")]
        public async Task<IActionResult> RemovePaper(string id)
        {
            Console.WriteLine($"deleted {id}");
            var paperlink = await _paperService.ReturnPaperLink(id);
            var response = await _paperService.RemovePaperAsync(id);
            if(response == true)
            {
                await _storage.DeleteAsync(paperlink);
                var deletedPaper = new PaperDeletedDto();
                deletedPaper.Id = id;
                deletedPaper.Event = "Delete_paper";
                //Send Async Message
                try
                {
                    _messageBusClient.DeletePaper(deletedPaper);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not send message async: {ex.Message}");
                }
                return Ok("Success");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("pdf")]
        [Authorize(Policy = "Public")]
        public async Task<ActionResult> addPaperWithPDF([FromForm] PaperWithFileDto paper)
        {
            Console.WriteLine("Adding Paper ... ");
            //Mapping the DTO
            Paper newPaper = new Paper();
            newPaper.Title = paper.Title;
            newPaper.Description = paper.Description;
            newPaper.Author = paper.Author;
            newPaper.PublishedYear = paper.PublishedYear;
            newPaper.Category = paper.category;
            newPaper.AuthorId = paper.AuthorId;
            newPaper.CiteCount = 0;

            var allowedExtensions = new[] { ".pdf" };
            var fileExtension = Path.GetExtension(paper.Pdf.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file extension. Only PDF files are allowed.");
            }

            // save to blob-storage
            BlobResponseDto? response = await _storage.UploadAsync(paper.Pdf);

            if (response.Error == true)
            {
                // We got an error during upload, return an error with details to the client
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                //Get the link
                newPaper.PaperLink = response.Blob.Uri;

                //Save to DB
                await _paperService.AddPaperAsync(newPaper);

                var publishPaper = _mapper.Map<PaperPublishedDto>(newPaper);
                publishPaper.Event = "Publish";

                //Send Async Message
                try
                {
                    _messageBusClient.PublishNewPaper(publishPaper);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not send message async: {ex.Message}");
                }

                return Ok(newPaper);
            }
        }

        [HttpDelete("author/{authorId}")]
        public async Task<IActionResult> RemovePapersByAuthorId(int authorId)
        {
            await _paperService.RemovePapersByAuthorIdAsync(authorId);
            return Ok();
        }
    }
}
