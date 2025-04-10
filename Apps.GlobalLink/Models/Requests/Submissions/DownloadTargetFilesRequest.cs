using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class DownloadTargetFilesRequest : SubmissionRequest
{ 
    [Display("Submission ID"), DataSource(typeof(ProcessedSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;
}
