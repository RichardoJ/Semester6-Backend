using CatalogNoSQL.Dto;

namespace CatalogNoSQL.Service
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
