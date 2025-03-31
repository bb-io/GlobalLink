using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses;

public class SearchUsersResponse(IEnumerable<UserResponse> users)
{
    public List<UserResponse> Users { get; set; } = users.ToList();

    [Display("Total count")]
    public double TotalCount { get; set; } = users.Count();
}
