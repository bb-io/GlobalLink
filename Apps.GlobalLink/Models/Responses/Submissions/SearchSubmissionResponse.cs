using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class SearchSubmissionResponse(IEnumerable<SubmissionResponse> submissions)
{
    [Display("Submissions")]
    public List<TrimmedSubmissionResponse> Submissions { get; set; } =  submissions.Select(x => new TrimmedSubmissionResponse(x)).ToList();

    [Display("Total count")]
    public int TotalCount { get; set; } = submissions.Count();
}
