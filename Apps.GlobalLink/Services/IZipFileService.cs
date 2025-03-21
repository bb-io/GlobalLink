using Blackbird.Applications.Sdk.Common.Files;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Apps.GlobalLink.Services;

public interface IZipFileService
{
    Task<List<FileReference>> ExtractZipFilesAsync(MemoryStream zipStream);
    string ExtractFileNameFromContentDisposition(string? contentDisposition);
}
