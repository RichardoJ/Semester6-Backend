using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublishService.Dto;
using PublishService.Model;
using PublishService.RabbitMQ;
using PublishService.Repository;
using PublishService.Service;

namespace PublishService.Controllers
{
    [Route("api/publish")]
    [ApiController]
    public class PaperController : ControllerBase
    {
        private readonly IPaperService _service;
        private readonly ICategoryRepository _category;
        private readonly IMapper _mapper;
        private readonly IAzureStorage _storage;
        private readonly IMessageBusClient _messageBusClient;

        public PaperController(IPaperService paperService, IMapper mapper, ICategoryRepository categoryRepository, IMessageBusClient messageBusClient, IAzureStorage azureStorage)
        {
            _service = paperService;
            _category = categoryRepository;
            _mapper = mapper;
            _storage = azureStorage;
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
        public async Task<ActionResult> addPaper(PaperDto paper)
        {
            Console.WriteLine("Adding Paper ... ");
            var paperCategory = paper.category;
            Category category = _category.GetCategory(paperCategory);
            PaperWriteDto paperWriteDto = new PaperWriteDto();
            paperWriteDto.Title = paper.Title;
            paperWriteDto.Description = paper.Description;
            paperWriteDto.Author = paper.Author;
            paperWriteDto.PublishedYear = paper.PublishedYear;
            paperWriteDto.category = category;
            paperWriteDto.AuthorId = paper.AuthorId;
            paperWriteDto.CiteCount = paper.CiteCount;
            paperWriteDto.PaperLink = paper.PaperLink;

            var savePaper = _mapper.Map<Model.Paper>(paperWriteDto);
            var check = _service.AddPaper(savePaper);
            if(check == true)
            {
                var publishPaper = _mapper.Map<PaperPublishedDto>(paperWriteDto);
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

                return Ok(savePaper);
            }
            else
            {
                return BadRequest("A paper with the same title already exists.");
            }

            
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult> updatePaper(PaperUpdateDto paper)
        {
            Console.WriteLine("Update Paper ... ");
            var paperCategory = paper.category;
            Category category = _category.GetCategory(paperCategory);
            Paper updatedPaper = new Paper();
            updatedPaper.Id = paper.Id;
            updatedPaper.Title = paper.Title;
            updatedPaper.Description = paper.Description;
            updatedPaper.AuthorId = paper.AuthorId;
            updatedPaper.Author = paper.Author;
            updatedPaper.Category = category;
            updatedPaper.PublishedYear = paper.PublishedYear;
            updatedPaper.CiteCount = paper.CiteCount;
            updatedPaper.PaperLink = paper.PaperLink;
            _service.UpdatePaper(updatedPaper);

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

            return Ok(updatedPaper);
        }

        [HttpDelete("{id}")]
        public ActionResult deletePaper(int id)
        {
            Console.WriteLine($"deleted {id}");
            _service.RemovePaper(id);
            if (_service.RemovePaper(id) == true)
            {
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
                return Ok(NotFound());
            }
        }

        [HttpPost]
        [Route("pdf")]
        public async Task<ActionResult> addPaperWithPDF([FromForm] PaperWithFileDto paper)
        {
            Console.WriteLine("Adding Paper ... ");
            //Mapping the DTO
            var paperCategory = paper.category;
            Category category = _category.GetCategory(paperCategory);
            PaperWriteDto paperWriteDto = new PaperWriteDto();
            paperWriteDto.Title = paper.Title;
            paperWriteDto.Description = paper.Description;
            paperWriteDto.Author = paper.Author;
            paperWriteDto.PublishedYear = paper.PublishedYear;
            paperWriteDto.category = category;
            paperWriteDto.AuthorId = paper.AuthorId;
            paperWriteDto.CiteCount = 0;

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
                paperWriteDto.PaperLink = response.Blob.Uri;
                var savePaper = _mapper.Map<Model.Paper>(paperWriteDto);

                //Save to DB
                _service.AddPaper(savePaper);

                var publishPaper = _mapper.Map<PaperPublishedDto>(paperWriteDto);
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

                return Ok(savePaper);
            }
        }
    }
}
