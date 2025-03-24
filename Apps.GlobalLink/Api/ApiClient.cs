using Apps.GlobalLink.Models.Dtos;
using Apps.GlobalLink.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.GlobalLink.Api;

public class ApiClient(IEnumerable<AuthenticationCredentialsProvider> credentials) : BlackBirdRestClient(new()
    {
        BaseUrl = new Uri(credentials.GetBaseUrl()),
    })
{
    protected override JsonSerializerSettings? JsonSettings => new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.None,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateParseHandling = DateParseHandling.None,
    };

    public async Task<List<T>> PaginateAsync<T>(RestRequest request)
    {
        var allResults = new List<T>();
        if (!request.Parameters.Any(p => p.Name == "pageSize"))
        {
            request.AddQueryParameter("pageSize", "200");
        }
        
        int currentPage = 0;
        int totalSize = 0;
        bool firstPage = true;
        
        do
        {
            if (!firstPage)
            {
                var existingPageParam = request.Parameters.FirstOrDefault(p => 
                    p.Name == "pageNumber" && p.Type == ParameterType.QueryString);
                if (existingPageParam != null)
                {
                    request.RemoveParameter(existingPageParam);
                }
                
                request.AddQueryParameter("pageNumber", currentPage.ToString());
            }
            
            firstPage = false;
            
            var response = await ExecuteWithErrorHandling(request);
            var pageResults = JsonConvert.DeserializeObject<List<T>>(response.Content!, JsonSettings);
        
            if (pageResults != null && pageResults.Count > 0)
            {
                allResults.AddRange(pageResults);
            }
            
            var pageSizeHeader = response.Headers?.FirstOrDefault(h => 
                h.Name?.Equals("page-size", StringComparison.OrdinalIgnoreCase) == true);
            
            var totalSizeHeader = response.Headers?.FirstOrDefault(h => 
                h.Name?.Equals("total-size", StringComparison.OrdinalIgnoreCase) == true);
            
            if (totalSizeHeader != null && int.TryParse(totalSizeHeader.Value?.ToString(), out int newTotalSize))
            {
                totalSize = newTotalSize;
            }
            
            currentPage++;
            if (allResults.Count >= totalSize || pageResults == null || pageResults.Count == 0)
            {
                break;
            }
        } while (true);
        
        return allResults;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<ErrorDto>(response.Content!);
        if(error is null)
        {
            return new PluginApplicationException(response.Content ?? response.ErrorMessage ?? $"Status code: {response.StatusCode}");
        }

        throw new PluginApplicationException(error.ToString());
    }
}
