using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Handlers.SubmissionHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions
{
    public class DownloadReferenceFilesRequest : SubmissionRequest
    {
        [Display("Submission ID"), DataSource(typeof(ProcessedSubmissionDataHandler))]
        public string SubmissionId { get; set; } = string.Empty;

        [Display("Include submission-level references")]
        public bool? SubmissionLevel { get; set; } = true;

        [Display("Language-level references")]
        public IEnumerable<string>? Languages { get; set; }
    }
}
