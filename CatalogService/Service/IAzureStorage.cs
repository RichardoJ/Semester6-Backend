using CatalogService.Dto;

namespace CatalogService.Service
{
    public interface IAzureStorage
    {
        Task<BlobResponseDto> UploadAsync(IFormFile file);

        Task<BlobDto> DownloadAsync(string blobFilename);

        Task<BlobResponseDto> DeleteAsync(string blobFilename);

        Task<List<BlobDto>> ListAsync();

        Task<byte[]> GetPdfFileAsync(string blobFilename);
    }
}
