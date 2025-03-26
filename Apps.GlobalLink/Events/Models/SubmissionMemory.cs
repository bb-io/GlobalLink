namespace Apps.GlobalLink.Events.Models;

public class SubmissionMemory
{
    public DateTime LastPollingTime { get; set; }
    public List<string> SubmissionIds { get; set; } = new();
}