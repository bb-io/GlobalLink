using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class MarkTargetsAsDeliveredResponse
{
    [Display("Successfully delivered target IDs")]
    public IEnumerable<string> SuccessfullyDeliveredTargetIds { get; set; } = Array.Empty<string>();

    [Display("Failed to deliver target IDs")]
    public IEnumerable<string> FailedToDeliverTargetIds { get; set; } = Array.Empty<string>();
}
