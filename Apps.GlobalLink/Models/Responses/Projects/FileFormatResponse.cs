using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Projects;

public class FileFormatResponse
{
    [Display("File format name")]
    public string Name { get; set; } = string.Empty;

    [Display("File format ID")]
    public string FileFormatId { get; set; } = string.Empty;

    [Display("Default")]
    public bool Default { get; set; }

    [Display("Enabled")]
    public bool Enabled { get; set; }

    [Display("Non parsable")]
    public bool NonParsable { get; set; }

    [Display("Target format")]
    public string? TargetFormat { get; set; }

    [Display("Default workflow definition ID")]
    public string? DefaultWorkflowDefinitionId { get; set; }

    [Display("Default workflow definition name")]
    public string? DefaultWorkflowDefinitionName { get; set; }

    [Display("MIME types")]
    public List<string> MimeTypes { get; set; } = new();
}
