using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Apps.GlobalLink.Utils;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class SubmissionResponse
{
    [Display("Submission ID")]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Project ID")]
    public string ProjectId { get; set; } = string.Empty;

    [Display("Submission name")]
    public string Name { get; set; } = string.Empty;

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Project name")]
    public string ProjectName { get; set; } = string.Empty;

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
    public DateTime DateStarted { get; set; }

    [Display("Date archived"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateArchived { get; set; }

    [Display("Date completed"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DateCompleted { get; set; }

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

    [Display("Original due date"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime OriginalDueDate { get; set; }

    [Display("Date quote requested"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateQuoteRequested { get; set; }

    [Display("Date canceled"), JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DateCanceled { get; set; }

    [DefinitionIgnore]
    public List<BatchInfo> BatchInfos { get; set; } = new();
}

public class UserInfo
{
    public string Locale { get; set; } = string.Empty;
    public bool SsoUser { get; set; }
    public bool Enabled { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long UserId { get; set; }
}

public class BatchInfo
{
    public string Name { get; set; } = string.Empty;
    public List<Target> Targets { get; set; } = new();
    public int WordCount { get; set; }
    public string? TargetFormat { get; set; }
    public long BatchId { get; set; }
    public List<PhaseInfo> PhaseInfos { get; set; } = new();
    public long WorkflowId { get; set; }
    public List<TargetLanguageInfo> TargetLanguageInfos { get; set; } = new();
    public string WorkflowName { get; set; } = string.Empty;
}

public class Target
{
    public List<TranslatableName> TranslatableNames { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public long DocumentId { get; set; }
    public long JobId { get; set; }
    public long SubmissionId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DueDate { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
    public long TargetId { get; set; }
    public string FileFormatName { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DateCompleted { get; set; }
    public int FileFormatId { get; set; }
    public string? CurrentPhase { get; set; }
}

public class TranslatableName
{
    public string PhaseName { get; set; } = string.Empty;
    public string TargetFileName { get; set; } = string.Empty;
}

public class PhaseInfo
{
    public string PhaseName { get; set; } = string.Empty;
    public List<string> AvailableEditors { get; set; } = new();
    public bool UseProtectedPayload { get; set; }
}

public class TargetLanguageInfo
{
    public string Operation { get; set; } = string.Empty;
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DueDate { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
    public string TargetLanguageDisplayName { get; set; } = string.Empty;
    public List<DueDateInfo> DueDateInfos { get; set; } = new();
}

public class DueDateInfo
{
    public bool Completed { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DueDate { get; set; }
}

public class TechTracking
{
    public string? AdaptorName { get; set; }
    public string? AdaptorVersion { get; set; }
    public string? ClientVersion { get; set; }
    public string? TechnologyProduct { get; set; }
}
