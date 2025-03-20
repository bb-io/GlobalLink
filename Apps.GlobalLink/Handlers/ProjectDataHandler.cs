using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Responses.Projects;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Handlers;

public class ProjectDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/v0/projects", Method.Get, Credentials);
        if(!string.IsNullOrEmpty(context.SearchString))
        {
            request.AddQueryParameter("projectName", context.SearchString);
        }

        var apiClient = new ApiClient(Credentials);

        var response = await apiClient.ExecuteWithErrorHandling<List<ProjectResponse>>(request);
        return response.Select(x => new DataSourceItem(x.ProjectId, x.Name)).ToList();
    }
}
