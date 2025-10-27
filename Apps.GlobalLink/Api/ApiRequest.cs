using Apps.GlobalLink.Models.Dtos;
using Apps.GlobalLink.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.GlobalLink.Api;

public class ApiRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> credentials)
    : BlackBirdRestRequest(resource, method, credentials)
{
    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var restClient = new RestClient(new Uri(creds.GetBaseUrl()));

        var basicAuthToken = creds.GetBasicAuthToken();
        var restRequest = new RestRequest("/oauth/token", Method.Post)
            .AddHeader("Authorization", $"Basic {basicAuthToken}")
            .AddParameter("grant_type", "password")
            .AddParameter("username", creds.GetUsername())
            .AddParameter("password", creds.GetPassword());

        var response = restClient.Execute(restRequest);
        if (response.IsSuccessStatusCode)
        {
            var token = JsonConvert.DeserializeObject<AccessTokenDto>(response.Content!);
            if (token != null)
            {
                this.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            }
            else
            {
                throw new Exception($"Failed to deserialize token. Response: {response.Content}");
            }
        }
        else
        {
            throw new Exception($"Failed to authenticate. Status code: {response.StatusCode}, Message: {response.Content}");
        }
    }
}
