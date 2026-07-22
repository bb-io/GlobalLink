using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class ReopenTargetFileResponse
{
    [Display("Reopen workflow ID")]
    public string ReopenWorkflowId { get; set; } = string.Empty;
}
