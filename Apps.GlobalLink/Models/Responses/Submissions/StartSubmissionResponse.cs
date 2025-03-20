using Apps.GlobalLink.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses.Submissions;

public class StartSubmissionResponse(StartSubmissionDto dto)
{
    [Display("Submission ID")]
    public string SubmissionId { get; set; } = dto.StartedSubmissionIds.FirstOrDefault().ToString() ?? string.Empty;
}
