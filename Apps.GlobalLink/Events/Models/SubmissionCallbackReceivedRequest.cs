using Apps.GlobalLink.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.GlobalLink.Events.Models;

public class SubmissionCallbackReceivedRequest
{
    [Display("Webhook scopes"), StaticDataSource(typeof(WebhookScopeStaticDataHandler))]
    public IEnumerable<string> WebhookScopes { get; set; } = [];
}
