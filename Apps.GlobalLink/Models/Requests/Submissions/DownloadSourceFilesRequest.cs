using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class DownloadSourceFilesRequest
{   
    [Display("Submission ID"), DataSource(typeof(SubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Phase name"), DataSource(typeof(PhaseNameDataHandler))]
    public string PhaseName { get; set; } = string.Empty;
}