using Kinematik.Application.Ports;
using Microsoft.AspNetCore.Http;

namespace Kinematik.FileStorage
{
    public class OnDiskFileStorageService: IFileStorageService
    {
        private const string STORAGE_FOLDER_NAME = "FileUploads";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private string StorageRootPath => Path.Combine(Directory.GetCurrentDirectory(), STORAGE_FOLDER_NAME);

        public OnDiskFileStorageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task StoreFileAsync(string relativePath, byte[] contents, CancellationToken cancellationToken = default)
        {
            string absoluteFilePath = this.GetAbsoluteStoragePath(relativePath)!;
            string absoluteFolderPath = Path.GetDirectoryName(absoluteFilePath)!;
            
            if (!Directory.Exists(absoluteFolderPath))
            {
                Directory.CreateDirectory(absoluteFolderPath);
            }

            using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
            {
                await stream.WriteAsync(contents, cancellationToken);
            }
        }

        public string? GetAbsoluteStoragePath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            return Path.Combine(this.StorageRootPath, relativePath);
        }

        public string? GetAccessingPath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            UriBuilder resultBuilder = new UriBuilder(
                _httpContextAccessor.HttpContext.Request.Scheme,
                _httpContextAccessor.HttpContext.Request.Host.Host
            );
            if (_httpContextAccessor.HttpContext.Request.Host.Port.HasValue)
            {
                resultBuilder.Port = _httpContextAccessor.HttpContext.Request.Host.Port!.Value;
            }

            resultBuilder.Path = Path.Combine(STORAGE_FOLDER_NAME, relativePath);

            return resultBuilder.ToString();
        }
    }
}