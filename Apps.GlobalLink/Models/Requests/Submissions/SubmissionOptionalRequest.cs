using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class SubmissionOptionalRequest
{
    [Display("Submission ID")]
    public string? SubmissionId { get; set; }
}
