using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class DownloadSourceFilesRequest : SubmissionRequest
{
    [Display("Phase name"), DataSource(typeof(PhaseNameDataHandler))]
    public string PhaseName { get; set; } = string.Empty;
}
