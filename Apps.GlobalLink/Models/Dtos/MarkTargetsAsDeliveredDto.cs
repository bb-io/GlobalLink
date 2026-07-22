namespace Apps.GlobalLink.Models.Dtos;

public class MarkTargetsAsDeliveredDto
{
    public long[] SuccessfullyDeliveredTargets { get; set; } = Array.Empty<long>();

    public long[] FailedToDeliverTargets { get; set; } = Array.Empty<long>();
}
