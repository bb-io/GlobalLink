using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class MarkTargetsAsDeliveredRequest : SubmissionRequest
{
    [Display("Submission ID"), DataSource(typeof(ProcessedSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Target IDs", Description = "Optional. If not specified, all targets currently in 'PROCESSED' status for this submission will be marked as delivered.")]
    public IEnumerable<string>? TargetIds { get; set; }
}
