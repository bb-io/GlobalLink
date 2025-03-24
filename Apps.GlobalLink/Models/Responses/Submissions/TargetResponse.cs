using Newtonsoft.Json;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class TargetResponse
{
    [JsonProperty("translatableNames")]
    public TranslatableName[] TranslatableNames { get; set; } = Array.Empty<TranslatableName>();

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("documentId")]
    public string DocumentId { get; set; } = string.Empty;

    [JsonProperty("jobId")]
    public string JobId { get; set; } = string.Empty;

    [JsonProperty("submissionId")]
    public string SubmissionId { get; set; } = string.Empty;

    [JsonProperty("documentName")]
    public string DocumentName { get; set; } = string.Empty;

    [JsonProperty("dueDate")]
    public long DueDate { get; set; }

    [JsonProperty("targetLanguage")]
    public string TargetLanguage { get; set; } = string.Empty;

    [JsonProperty("targetId")]
    public string TargetId { get; set; } = string.Empty;

    [JsonProperty("fileFormatName")]
    public string FileFormatName { get; set; } = string.Empty;

    [JsonProperty("sourceLanguage")]
    public string SourceLanguage { get; set; } = string.Empty;

    [JsonProperty("dateCompleted")]
    public long DateCompleted { get; set; }

    [JsonProperty("fileFormatId")]
    public string FileFormatId { get; set; } = string.Empty;

    [JsonProperty("currentPhase")]
    public string CurrentPhase { get; set; } = string.Empty;
}
