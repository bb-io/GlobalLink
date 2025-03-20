using System;
using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class ClaimSubmissionRequest
{
    [Display("Submission ID"), DataSource(typeof(ClaimableSubmissionDataHandler))]
    public string SubmissionId { get; set; } = string.Empty;

    [Display("Phase name"), DataSource(typeof(PhaseNameDataHandler))]
    public string PhaseName { get; set; } = string.Empty;

    [Display("Target languages"), DataSource(typeof(TargetLanguageDataHandler))]
    public IEnumerable<string> TargetLanguages { get; set; } = Array.Empty<string>();
}
