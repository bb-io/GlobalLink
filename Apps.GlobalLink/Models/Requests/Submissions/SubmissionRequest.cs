using System;
using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class SubmissionRequest
{
    [Display("Submission ID"), DataSource(typeof(SubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;
}
