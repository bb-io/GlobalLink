using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class SearchSubmissionResponse(IEnumerable<SubmissionResponse> submissions)
{
    [Display("Submissions")]
    public List<SubmissionResponse> Submissions { get; set; } =  submissions.ToList();

    [Display("Total count")]
    public int TotalCount { get; set; } = submissions.Count();
}
