using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class DownloadTargetFilesResponse
{
    [Display("Target files")]
    public IEnumerable<DownloadFileGroupResponse> TargetFiles { get; set; } = Array.Empty<DownloadFileGroupResponse>();
}
