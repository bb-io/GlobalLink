using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Requests.Projects;
using Apps.GlobalLink.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class SourceLanguageDataHandler(InvocationContext invocationContext, [ActionParameter] ProjectRequest projectRequest)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(projectRequest.ProjectId))
        {
            throw new ArgumentException("'Project ID' is required. Please input this input first.");
        }

        var apiClient = new ApiClient(Credentials);
        var apiRequest = new ApiRequest($"/rest/v0/projects/{projectRequest.ProjectId}/languagedirections", Method.Get, Credentials);

        var response = await apiClient.PaginateAsync<LanguageResponse>(apiRequest);
        return response.DistinctBy(x => x.SourceLanguage)
            .Where(x => string.IsNullOrEmpty(context.SearchString) || x.SourceLanguageDisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x.SourceLanguage, x.SourceLanguageDisplayName))
            .ToList();
    }
}