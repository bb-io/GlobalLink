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
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject(response.Content!);
        var errorMessage = "";

        throw new PluginApplicationException(errorMessage);
    }
}
