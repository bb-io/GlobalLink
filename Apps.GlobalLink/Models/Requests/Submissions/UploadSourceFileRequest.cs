

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

    [Display("File format name", Description = "Name of the GlobalLink parser that processes the source file, e.g. 'MSWORD_TXLF'. By default we detect it from the file type and the parsers configured in the project, so only set this if the project has several parsers for the same file type or if you want to override our detection. Ignored for non-parsable batches."),
     DataSource(typeof(FileFormatDataHandler))]
    public string? FileFormatName { get; set; }
}
