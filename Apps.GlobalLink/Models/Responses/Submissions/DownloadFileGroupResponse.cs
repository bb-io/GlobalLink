using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class DownloadFileGroupResponse
{
    [Display("Source language")]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language")]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("File")]
    public FileReference File { get; set; } = default!;
}
