using Newtonsoft.Json;
using Apps.GlobalLink.Utils;
using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class TrimmedSubmissionResponse
{
    [Display("Submission ID")]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Project ID")]
    public string ProjectId { get; set; } = string.Empty;

    [Display("Submission name")]
    public string Name { get; set; } = string.Empty;

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Submission status")]
    public string Status { get; set; } = string.Empty;

    [Display("Source language")]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target languages")]
    public List<string> TargetLanguages { get; set; } = new();

    [Display("Created at"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DateCreated { get; set; }

    [Display("Due date"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DueDate { get; set; }

    [Display("Date started"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateStarted { get; set; }

    [Display("Date archived"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateArchived { get; set; }

    [Display("Date completed"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateCompleted { get; set; }

    [Display("Quote enabled")]
    public bool QuoteEnabled { get; set; }

    [Display("Started")]
    public bool Started { get; set; }

    [Display("Template")]
    public bool Template { get; set; }

    [Display("Auto quote")]
    public bool AutoQuote { get; set; }

    [Display("Claim scope")]
    public string ClaimScope { get; set; } = string.Empty;

    [Display("Favorite")]
    public bool Favorite { get; set; }

    [Display("File count")]
    public int FileCount { get; set; }

    [Display("Instructions")]
    public string? Instructions { get; set; }

    [Display("Project client name")]
    public string? ProjectAClientName { get; set; }

    [Display("Project client ID")]
    public string? ProjectAClientId { get; set; }

    [Display("Background")]
    public string? Background { get; set; }

    [Display("Date quote requested"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateQuoteRequested { get; set; }

    [Display("Date canceled"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateCanceled { get; set; }

    [Display("Owners")]
    public List<UserResponse> Owners { get; set; } = new();

    public TrimmedSubmissionResponse()
    { }

    public TrimmedSubmissionResponse(SubmissionResponse submissionResponse)
    {
        SubmissionId = submissionResponse.SubmissionId;
        ProjectId = submissionResponse.ProjectId;
        Name = submissionResponse.Name;
        Description = submissionResponse.Description;
        Status = submissionResponse.Status;
        SourceLanguage = submissionResponse.SourceLanguage;
        TargetLanguages = submissionResponse.TargetLanguages;
        DateCreated = submissionResponse.DateCreated;
        DueDate = submissionResponse.DueDate;
        DateStarted = submissionResponse.DateStarted;
        DateArchived = submissionResponse.DateArchived;
        DateCompleted = submissionResponse.DateCompleted;
        QuoteEnabled = submissionResponse.QuoteEnabled;
        Started = submissionResponse.Started;
        Template = submissionResponse.Template;
        AutoQuote = submissionResponse.AutoQuote;
        ClaimScope = submissionResponse.ClaimScope;
        Favorite = submissionResponse.Favorite;
        FileCount = submissionResponse.FileCount;
        Instructions = submissionResponse.Instructions;
        ProjectAClientName = submissionResponse.ProjectAClientName;
        ProjectAClientId = submissionResponse.ProjectAClientId;
        Background = submissionResponse.Background;
        DateQuoteRequested = submissionResponse.DateQuoteRequested;
        DateCanceled = submissionResponse.DateCanceled;
        Owners = submissionResponse.Owners;
    }
}
