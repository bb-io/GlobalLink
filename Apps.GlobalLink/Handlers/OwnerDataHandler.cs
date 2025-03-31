using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class OwnerDataHandler(InvocationContext invocationContext, [ActionParameter] SearchSubmissionsRequest submissionRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(submissionRequest.ProjectId))
        {
            throw new ArgumentException("'Project ID' is required. Please input this input first.");
        }

        var request = new ApiRequest($"/rest/v0/projects/{submissionRequest.ProjectId}/users/org", Method.Get, Credentials);
        var apiClient = new ApiClient(Credentials);

        var response = await apiClient.PaginateAsync<UserResponse>(request);
        return response.Where(x => string.IsNullOrEmpty(context.SearchString) || x.UserName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.UserId, x.UserName))
            .ToList();
    }
}