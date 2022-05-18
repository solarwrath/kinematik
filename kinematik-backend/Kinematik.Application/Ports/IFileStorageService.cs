namespace Kinematik.Application.Ports
{
    public interface IFileStorageService
    {
        Task StoreFileAsync(string relativePath, byte[] contents, CancellationToken cancellationToken = default);
        string GetAccessingPath(string? relativePath);
    }
}