using Blackbird.Applications.Sdk.Common;

namespace Apps.GlobalLink.Models.Responses;

public class UserResponse
{    
    [Display("User ID")]
    public string UserId { get; set; } = string.Empty;
    
    [Display("First name")]
    public string FirstName { get; set; } = string.Empty;
    
    [Display("Last name")]
    public string LastName { get; set; } = string.Empty;
    
    [Display("Email")]
    public string Email { get; set; } = string.Empty;

    [Display("Enabled")]
    public bool Enabled { get; set; }
    
    [Display("Username")]
    public string UserName { get; set; } = string.Empty;
    
    [Display("User type")]
    public string UserType { get; set; } = string.Empty;
    
    [Display("SSO user")]
    public bool SsoUser { get; set; }
}
