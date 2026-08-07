using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Projects;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class FileFormatDataHandler(InvocationContext invocationContext, [ActionParameter] SubmissionRequest submissionRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(submissionRequest.SubmissionId))
        {
            throw new ArgumentException("'Submission ID' is required. Please input this input first.");
        }

        var apiClient = new ApiClient(Credentials);

        var submissionApiRequest = new ApiRequest($"/rest/v0/submissions/{submissionRequest.SubmissionId}", Method.Get, Credentials);
        var submission = await apiClient.ExecuteWithErrorHandling<SubmissionResponse>(submissionApiRequest);

        var fileFormatsApiRequest = new ApiRequest($"/rest/v0/projects/{submission.ProjectId}/fileformats", Method.Get, Credentials);
        var fileFormats = await apiClient.ExecuteWithErrorHandling<List<FileFormatResponse>>(fileFormatsApiRequest);

        return fileFormats
            .Where(x => x.Enabled && !x.NonParsable && !string.IsNullOrEmpty(x.Name))
            .Where(x => string.IsNullOrEmpty(context.SearchString) || x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.Name, x.Name))
            .ToList();
    }
}
