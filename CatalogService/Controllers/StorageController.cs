using CatalogService.Dto;
using CatalogService.Service;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/catalog/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IAzureStorage _storage;

        public StorageController(IAzureStorage storage)
        {
            _storage = storage;
        }

        [HttpGet(nameof(Get))]
        public async Task<IActionResult> Get()
        {
            // Get all files at the Azure Storage Location and return them
            List<BlobDto>? files = await _storage.ListAsync();

            // Returns an empty array if no files are present at the storage container
            return StatusCode(StatusCodes.Status200OK, files);
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            BlobDto? file = await _storage.DownloadAsync(filename);

            // Check if file was found
            if (file == null)
            {
                // Was not, return error message to client
                return StatusCode(StatusCodes.Status500InternalServerError, $"File {filename} could not be downloaded.");
            }
            else
            {
                // File was found, return it to client
                return File(file.Content, file.ContentType, file.Name);
            }
        }

        [HttpGet("pdf/{filename}")]
        public async Task<IActionResult> GetPdfFile(string filename)
        {
            var pdfFile = await _storage.GetPdfFileAsync(filename);
            return File(pdfFile, "application/pdf");
        }
    }
}
