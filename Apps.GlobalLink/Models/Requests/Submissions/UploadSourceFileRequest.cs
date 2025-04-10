

using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class UploadSourceFileRequest
{
    [Display("Submission ID"), DataSource(typeof(UploadSourceSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;

    public FileReference File { get; set; } = default!;

    [Display("Batch name", Description = "If not provided we will use 'Batch1' value"), DataSource(typeof(BatchDataHandler))]
    public string? BatchName { get; set; } 
}
