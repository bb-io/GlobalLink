using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Responses.Projects;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using RestSharp;

namespace Apps.GlobalLink.Services;

public class FileFormatResolver(IEnumerable<AuthenticationCredentialsProvider> credentials) : IFileFormatResolver
{
    private const string OctetStreamMimeType = "application/octet-stream";
    private const string ZipExtension = ".zip";

    private readonly AuthenticationCredentialsProvider[] credentials = credentials.ToArray();

    public async Task<string?> ResolveFileFormatNameAsync(string projectId, FileReference file)
    {
        if (file.Name.EndsWith(ZipExtension, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var candidateMimeTypes = GetCandidateMimeTypes(file);
        if (candidateMimeTypes.Count == 0)
        {
            return null;
        }

        var fileFormats = await GetFileFormatsAsync(projectId);
        return FileFormatMatcher.Match(fileFormats, candidateMimeTypes);
    }

    private async Task<List<FileFormatResponse>> GetFileFormatsAsync(string projectId)
    {
        var apiClient = new ApiClient(credentials);
        var apiRequest = new ApiRequest($"/rest/v0/projects/{projectId}/fileformats", Method.Get, credentials);

        return await apiClient.ExecuteWithErrorHandling<List<FileFormatResponse>>(apiRequest);
    }
    
    private static List<string> GetCandidateMimeTypes(FileReference file)
    {
        var candidates = new List<string>();

        var mimeTypeByName = MimeTypes.GetMimeType(file.Name);
        if (!string.Equals(mimeTypeByName, OctetStreamMimeType, StringComparison.OrdinalIgnoreCase))
        {
            candidates.Add(mimeTypeByName);
        }

        if (!string.IsNullOrEmpty(file.ContentType)
            && !string.Equals(file.ContentType, OctetStreamMimeType, StringComparison.OrdinalIgnoreCase)
            && !candidates.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            candidates.Add(file.ContentType);
        }

        return candidates;
    }
}
