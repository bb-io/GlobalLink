using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class UploadTargetFileRequest : SubmissionRequest
{
    [Display("Target file")]
    public FileReference File { get; set; } = default!;
}
