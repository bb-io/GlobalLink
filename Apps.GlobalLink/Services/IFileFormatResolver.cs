using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Services;

public interface IFileFormatResolver
{
    Task<string?> ResolveFileFormatNameAsync(string projectId, FileReference file);
}
