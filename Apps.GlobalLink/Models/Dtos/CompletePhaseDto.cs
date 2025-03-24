namespace Apps.GlobalLink.Models.Dtos;

public class CompletePhaseDto
{
    public long[] CompletedTargets { get; set; } = Array.Empty<long>();
    
    public long[] FailedTargets { get; set; } = Array.Empty<long>();
}
