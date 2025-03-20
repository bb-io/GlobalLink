using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class UploadSourceFileResponse
{
    [Display("Process ID")]
    public string ProcessId { get; set; } = string.Empty;

    [Display("Submission ID")]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Documents")]
    public List<DocumentInfo> DocumentIds { get; set; } = new();
}

public class DocumentInfo
{
    [Display("File name")]
    public string Name { get; set; } = string.Empty;

    [Display("Document ID")] 
    public string DocumentId { get; set; } = string.Empty;
}
