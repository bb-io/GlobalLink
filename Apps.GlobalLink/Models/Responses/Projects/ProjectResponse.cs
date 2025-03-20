using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Projects;

public class ProjectResponse
{
    [Display("Project ID")]
    public string ProjectId { get; set; } = string.Empty;

    [Display("Project name")]
    public string Name { get; set; } = string.Empty;

    [Display("Enabled")]
    public bool Enabled { get; set; }

    [Display("Quote enabled")]
    public bool QuoteEnabled { get; set; }

    [Display("Auto start submission")]
    public bool AutoStartSubmission { get; set; }

    [Display("Display MSLA level")]
    public bool DisplayMslaLevel { get; set; }

    [Display("Quote scope")]
    public string? QuoteScope { get; set; }

    [Display("Auto quote")]
    public bool AutoQuote { get; set; }

    [Display("Organization ID")]
    public string OrganizationId { get; set; } = string.Empty;

    [Display("Media assets enabled")]
    public bool MediaAssetsEnabled { get; set; }

    [Display("Cost scope ID")]
    public string CostScopeId { get; set; } = string.Empty;

    [Display("Short code")]
    public string ShortCode { get; set; } = string.Empty;

    [Display("Default workflow definition ID")]
    public string DefaultWorkflowDefinitionId { get; set; } = string.Empty;
}
