using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class PhaseNameDataHandler(InvocationContext invocationContext, [ActionParameter] SubmissionRequest submissionRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(submissionRequest.SubmissionId))
        {
            throw new ArgumentException("'Submission ID' is required. Please input this input first.");
        }

        var request = new ApiRequest($"/rest/v0/submissions/{submissionRequest.SubmissionId}/phases", Method.Get, Credentials);
        var apiClient = new ApiClient(Credentials);

        var response = await apiClient.PaginateAsync<PhaseResponse>(request);
        return response.
            DistinctBy(x => x.CurrentPhase)
            .Where(x => string.IsNullOrEmpty(context.SearchString) || x.CurrentPhase.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.CurrentPhase, x.CurrentPhase)).ToList();
    }
}