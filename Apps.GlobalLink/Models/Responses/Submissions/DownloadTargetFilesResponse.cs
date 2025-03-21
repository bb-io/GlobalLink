using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class DownloadTargetFilesResponse
{
    [Display("Target files")]
    public IEnumerable<DownloadTargetFileGroupResponse> TargetFiles { get; set; } = Array.Empty<DownloadTargetFileGroupResponse>();
}

public class DownloadTargetFileGroupResponse 
{
    [Display("Source language")]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language")]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Target file")]
    public FileReference File { get; set; } = default!;
}
