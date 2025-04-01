using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class SubmissionDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public virtual List<string> AllowedStatuses { get; } = new();

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/v0/submissions", Method.Get, Credentials);
        if(!string.IsNullOrEmpty(context.SearchString))
        {
            request.AddQueryParameter("name", context.SearchString);
        }

        if (AllowedStatuses.Count > 0)
        {
            request.AddQueryParameter("statuses", string.Join(",", AllowedStatuses));
        }

        var apiClient = new ApiClient(Credentials);

        var response = await apiClient.PaginateAsync<SubmissionResponse>(request);
        return response.Select(x => new DataSourceItem(x.SubmissionId, x.Name)).ToList();
    }
}
