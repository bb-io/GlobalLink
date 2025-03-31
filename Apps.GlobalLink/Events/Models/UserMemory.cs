namespace Apps.GlobalLink.Events.Models;

public class UserMemory
{
    public DateTime LastPollingTime { get; set; }
    public List<string> UserIds { get; set; } = new();
}
