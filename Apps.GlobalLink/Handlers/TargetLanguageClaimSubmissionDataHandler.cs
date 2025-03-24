using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Requests.Projects;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class TargetLanguageClaimSubmissionDataHandler(InvocationContext invocationContext, [ActionParameter] ClaimSubmissionRequest claimSubmissionRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(claimSubmissionRequest.SubmissionId))
        {
            throw new ArgumentException("'Submission ID' is required. Please input this input first.");
        }
        
        var apiClient = new ApiClient(Credentials);
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{claimSubmissionRequest.SubmissionId}", Method.Get, Credentials);

        var response = await apiClient.PaginateAsync<SubmissionResponse>(apiRequest);
        return response.Where(x => string.IsNullOrEmpty(context.SearchString) || x.TargetLanguageDisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.TargetLanguage, x.TargetLanguageDisplayName))
            .ToList();
    }
}
