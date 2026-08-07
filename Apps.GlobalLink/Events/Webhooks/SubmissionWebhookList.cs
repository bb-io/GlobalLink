using System.Net;
using Apps.GlobalLink.Api;
using Apps.GlobalLink.Constants;
using Apps.GlobalLink.Events.Webhooks.Handlers;
using Apps.GlobalLink.Events.Webhooks.Models;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.GlobalLink.Events.Webhooks;

[WebhookList("Submission (webhooks)")]
public class SubmissionWebhookList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Webhook("On submission completed", typeof(SubmissionCompletedHandler),
        Description = "Triggered as soon as the specified submission is completed. Only one webhook can watch a " +
                      "submission at a time.")]
    public Task<WebhookResponse<SubmissionResponse>> OnSubmissionCompletedAsync(WebhookRequest webhookRequest,
        [WebhookParameter(true)] SubmissionRequest submissionRequest)
        => HandleWebhookAsync(webhookRequest, submissionRequest, ScopeConstants.SubmissionCompleted);

    [Webhook("On submission cancelled", typeof(SubmissionCancelledHandler),
        Description = "Triggered as soon as the specified submission is cancelled. Only one webhook can watch a " +
                      "submission at a time.")]
    public Task<WebhookResponse<SubmissionResponse>> OnSubmissionCancelledAsync(WebhookRequest webhookRequest,
        [WebhookParameter(true)] SubmissionRequest submissionRequest)
        => HandleWebhookAsync(webhookRequest, submissionRequest, ScopeConstants.SubmissionCancelled);

    private async Task<WebhookResponse<SubmissionResponse>> HandleWebhookAsync(WebhookRequest webhookRequest,
        SubmissionRequest submissionRequest, string scope)
    {
        var payload = JsonConvert.DeserializeObject<SubmissionWebhookPayload>(webhookRequest.Body.ToString()!);
        if (payload is null)
        {
            throw new InvalidCastException(nameof(webhookRequest.Body));
        }
        
        if (!string.Equals(payload.EventCode, scope, StringComparison.OrdinalIgnoreCase))
        {
            return Preflight();
        }

        var submissionId = payload.SubmissionId.ToString();
        if (!string.IsNullOrWhiteSpace(submissionRequest.SubmissionId) && !string.Equals(submissionRequest.SubmissionId, submissionId, StringComparison.OrdinalIgnoreCase))
        {
            return Preflight();
        }

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}", Method.Get, Credentials);
        var submission = await Client.ExecuteWithErrorHandling<SubmissionResponse>(apiRequest);

        return new WebhookResponse<SubmissionResponse>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = submission,
            ReceivedWebhookRequestType = WebhookRequestType.Default
        };
    }

    private static WebhookResponse<SubmissionResponse> Preflight() => new()
    {
        HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
        Result = null,
        ReceivedWebhookRequestType = WebhookRequestType.Preflight
    };
}
