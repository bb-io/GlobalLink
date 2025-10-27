using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions
{
    public class DownloadReferenceFilesResponse
    {
        [Display("Reference files")]
        public IEnumerable<DownloadFileGroupResponse> ReferenceFiles { get; set; } = Array.Empty<DownloadFileGroupResponse>();
    }
}
