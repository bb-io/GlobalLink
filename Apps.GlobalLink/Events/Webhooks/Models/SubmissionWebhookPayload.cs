using Newtonsoft.Json;

namespace Apps.GlobalLink.Events.Webhooks.Models;

public class SubmissionWebhookPayload
{
    [JsonProperty("eventCode")]
    public string EventCode { get; set; } = string.Empty;

    [JsonProperty("submissionId")]
    public long SubmissionId { get; set; }

    [JsonProperty("submissionName")]
    public string? SubmissionName { get; set; }
}
