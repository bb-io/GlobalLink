using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class DownloadSourceFilesResponse
{
    [Display("Source files")]
    public IEnumerable<DownloadFileGroupResponse> SourceFiles { get; set; } = Array.Empty<DownloadFileGroupResponse>();
}
