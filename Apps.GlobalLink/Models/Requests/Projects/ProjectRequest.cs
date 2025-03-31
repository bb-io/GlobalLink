using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Models.Requests.Projects;

public class ProjectRequest
{
    [Display("Project ID"), DataSource(typeof(ProjectDataHandler))]
    public virtual string ProjectId { get; set; } = string.Empty;
}
