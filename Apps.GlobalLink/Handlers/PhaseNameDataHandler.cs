using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class PhaseNameDataHandler(InvocationContext invocationContext, [ActionParameter] ClaimSubmissionRequest claimSubmissionRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(claimSubmissionRequest.SubmissionId))
        {
            throw new ArgumentException("'Submission ID' is required. Please input this input first.");
        }

        var request = new ApiRequest("/rest/v0/submissions/claimable", Method.Get, Credentials);
        var apiClient = new ApiClient(Credentials);

        var response = await apiClient.ExecuteWithErrorHandling<List<ClaimableSubmissionResponse>>(request);
        var specificSubmission = response.FirstOrDefault(x => x.SubmissionId == claimSubmissionRequest.SubmissionId);
        if (specificSubmission == null)
        {
            throw new ArgumentException($"No submission found with ID: {claimSubmissionRequest.SubmissionId}");
        }

        return specificSubmission.Languages
            .Where(x => string.IsNullOrEmpty(context.SearchString) || x.PhaseName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.PhaseName, x.PhaseName)).ToList();
    }
}