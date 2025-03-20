using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses;

public class WorkflowResponse
{
    [Display("Workflow ID")]
    public string Id { get; set; } = string.Empty;

    [Display("Workflow name")]
    public string Name { get; set; } = string.Empty;
}
