using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class ClaimableSubmissionResponse
{
    [Display("Submission ID")]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Submission name")]
    public string SubmissionName { get; set; } = string.Empty;

    [Display("Languages")]
    public IEnumerable<LanguageInfo> Languages { get; set; } = Array.Empty<LanguageInfo>();

    [Display("Claim level")]
    public string ClaimLevel { get; set; } = string.Empty;
}

public class LanguageInfo
{
    [Display("Phase name")]
    public string PhaseName { get; set; } = string.Empty;

    [Display("Source language code")]
    public string SourceLanguageCode { get; set; } = string.Empty;

    [Display("Source language name")]
    public string SourceLanguageName { get; set; } = string.Empty;

    [Display("Target language code")]
    public string TargetLanguageCode { get; set; } = string.Empty;

    [Display("Target language name")]
    public string TargetLanguageName { get; set; } = string.Empty;
}
