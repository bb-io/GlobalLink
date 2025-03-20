using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses;

public class LanguageResponse
{
    [Display("Default")]
    public bool Default { get; set; }

    [Display("Target language code")]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Source language code")]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language")]
    public string TargetLanguageDisplayName { get; set; } = string.Empty;

    [Display("Source Language")]
    public string SourceLanguageDisplayName { get; set; } = string.Empty;
}
