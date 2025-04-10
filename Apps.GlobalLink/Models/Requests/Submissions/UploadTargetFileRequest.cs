using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class UploadTargetFileRequest
{    
    [Display("Submission ID"), DataSource(typeof(UploadTargetSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Target file")]
    public FileReference File { get; set; } = default!;
}
