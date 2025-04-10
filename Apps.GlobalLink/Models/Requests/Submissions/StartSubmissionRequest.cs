using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class StartSubmissionRequest
{
    [Display("Submission ID"), DataSource(typeof(WaitingSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;
}
