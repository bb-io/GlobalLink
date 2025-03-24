using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class UploadReferenceFileRequest : SubmissionRequest 
{
    public FileReference File { get; set; } = default!;
}
