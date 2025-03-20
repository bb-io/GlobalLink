using System;
using Apps.GlobalLink.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.GlobalLink.Utils;

public static class AuthenticationCredentialsProviderExtensions
{
    public static string GetBaseUrl(this IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var baseUrlKeyValue = creds.Get(CredNames.BaseUrl);
        if(string.IsNullOrEmpty(baseUrlKeyValue.Value))
        {
            throw new Exception("Base URL is not provided. Please provide a valid base URL.");
        }

        return baseUrlKeyValue.Value.TrimEnd('/') + "/PD";
    }

    public static string GetUsername(this IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var usernameKeyValue = creds.Get(CredNames.Username);
        if(string.IsNullOrEmpty(usernameKeyValue.Value))
        {
            throw new Exception("Username is not provided. Please provide a valid username.");
        }

        return usernameKeyValue.Value;
    }

    public static string GetPassword(this IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var passwordKeyValue = creds.Get(CredNames.Password);
        if(string.IsNullOrEmpty(passwordKeyValue.Value))
        {
            throw new Exception("Password is not provided. Please provide a valid password.");
        }

        return passwordKeyValue.Value;
    }

    public static string GetBasicAuthToken(this IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var basicAuthTokenKeyValue = creds.Get(CredNames.BasicAuthToken);
        if(string.IsNullOrEmpty(basicAuthTokenKeyValue.Value))
        {
            throw new Exception("Basic Auth Token is not provided. Please provide a valid Basic Auth Token.");
        }

        return basicAuthTokenKeyValue.Value;
    }
}
