namespace Apps.GlobalLink.Models.Responses.Submissions;

public class SearchSubmissionResponse(IEnumerable<SubmissionResponse> submissions)
{
    public List<SubmissionResponse> Submissions { get; set; } =  submissions.ToList();

    public int TotalCount { get; set; } = submissions.Count();
}
