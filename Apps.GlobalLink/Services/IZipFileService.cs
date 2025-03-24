using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Services;

public interface IZipFileService
{
    Task<List<FileReference>> ExtractZipFilesAsync(MemoryStream zipStream);
    string ExtractFileNameFromContentDisposition(string? contentDisposition);
}
