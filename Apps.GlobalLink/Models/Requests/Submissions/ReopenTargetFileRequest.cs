using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class ReopenTargetFileRequest
{
    [Display("Submission ID"), DataSource(typeof(ProcessedSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Updated target file", Description = "The file name must not be modified in any way. GLE uses it to identify the target uniquely.")]
    public FileReference File { get; set; } = default!;

    [Display("Extract archive", Description = "Set to true if the uploaded file is a zip archive containing multiple updated targets.")]
    public bool? ExtractArchive { get; set; }

    [Display("Reopen workflow ID", Description = "Optional. Required only if the submission's workflow has more than one reopen workflow linked to it.")]
    public string? ReopenWorkflowId { get; set; }
}
