using Apps.GlobalLink.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.GlobalLink.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredNames.BaseUrl) 
                { 
                    DisplayName = "Base URL", 
                    Sensitive = false,
                    Description = "The base URL for the GlobalLink API. Expected format: `https://api.globallink.com`"
                },
                new(CredNames.Username) 
                { 
                    DisplayName = "Username", 
                    Sensitive = false,
                    Description = "The username of user for the GlobalLink API."
                },
                new(CredNames.Password) 
                { 
                    DisplayName = "Password", 
                    Sensitive = true,
                    Description = "The password of user for the GlobalLink API."
                },
                new(CredNames.BasicAuthToken) 
                { 
                    DisplayName = "Basic Auth Token", 
                    Sensitive = true,
                    Description = "The Basic Auth Token for the GlobalLink API."
                }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values) => values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();
}
