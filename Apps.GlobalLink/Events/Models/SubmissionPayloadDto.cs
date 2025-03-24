using Newtonsoft.Json;

namespace Apps.GlobalLink.Events.Models;

public class SubmissionPayloadDto
{
    [JsonProperty("documentIds")]
    public long[] DocumentIds { get; set; } = Array.Empty<long>();

    [JsonProperty("targetIds")]
    public long[] TargetIds { get; set; } = Array.Empty<long>();

    [JsonProperty("eventTime")]
    public long EventTime { get; set; }

    [JsonProperty("pdUrl")]
    public string PdUrl { get; set; } = string.Empty;

    [JsonProperty("submissionId")]
    public string SubmissionId { get; set; } = string.Empty;

    [JsonProperty("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonProperty("submissionName")]
    public string SubmissionName { get; set; } = string.Empty;

    [JsonProperty("eventCode")]
    public string EventCode { get; set; } = string.Empty;
}
