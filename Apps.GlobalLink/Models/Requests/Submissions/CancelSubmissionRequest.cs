using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class CancelSubmissionRequest
{
    [Display("Submission ID"), DataSource(typeof(NonCancelledSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;
}
