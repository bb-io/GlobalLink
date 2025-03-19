using System;

namespace Apps.GlobalLink.Models.Dtos;

public class AccessTokenDto
{
    [Newtonsoft.Json.JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("sub")]
    public string Sub { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("aud")]
    public string Aud { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("scope")]
    public string Scope { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("token_type")]
    public string TokenType { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    
    [Newtonsoft.Json.JsonProperty("authorities")]
    public string Authorities { get; set; } = string.Empty;
    
    [Newtonsoft.Json.JsonProperty("jti")]
    public string Jti { get; set; } = string.Empty;
}
