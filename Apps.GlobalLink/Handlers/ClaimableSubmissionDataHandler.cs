using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class ClaimableSubmissionDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/v0/submissions/claimable", Method.Get, Credentials);
        var apiClient = new ApiClient(Credentials);

        var response = await apiClient.PaginateAsync<ClaimableSubmissionResponse>(request);
        return response
            .Where(x => string.IsNullOrEmpty(context.SearchString) || x.SubmissionName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.SubmissionId, x.SubmissionName)).ToList();
    }
}
