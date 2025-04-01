using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class SearchSubmissionsRequest
{
    [Display("Due date from")]
    public DateTime? DueDateFrom { get; set; }
    
    [Display("Due date to")]
    public DateTime? DueDateTo { get; set; }
    
    [Display("Date started from")]
    public DateTime? DateStartedFrom { get; set; }
    
    [Display("Date started to")]
    public DateTime? DateStartedTo { get; set; }
    
    [Display("Submission status"), StaticDataSource(typeof(SubmissionStatusStaticDataHandler))]
    public string? Status { get; set; }

    [Display("Project ID", Description = "Used only for Owner data handler. It will no affect on action functionality."), DataSource(typeof(ProjectDataHandler))]
    public string? ProjectId { get; set; }
    
    [Display("Owner ID"), DataSource(typeof(OwnerDataHandler))]
    public string? OwnerId { get; set; }

    [Display("Exclude submissions with owners assigned")]
    public bool? ExcludeSubmissionsWithOwnersAssigned { get; set; }
}