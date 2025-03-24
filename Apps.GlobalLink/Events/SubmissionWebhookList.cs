using Apps.GlobalLink.Api;
using Apps.GlobalLink.Constants;
using Apps.GlobalLink.Events.Models;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.GlobalLink.Events;

[WebhookList]
public class SubmissionWebhookList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Webhook("On submission completed", Description = "Triggered when a submission is completed. This webhook may not be received immediately after completion and could be delayed.")]
    public async Task<WebhookResponse<SubmissionResponse>> OnSubmissionCompleted(WebhookRequest webhookRequest) =>
        await HandleWebhookRequest(webhookRequest, ScopeConstants.SubmissionCompleted);

    [Webhook("On submission cancelled", Description = "Triggered when a submission is cancelled. This webhook may not be received immediately after cancellation and could be delayed.")]
    public async Task<WebhookResponse<SubmissionResponse>> OnSubmissionCancelled(WebhookRequest webhookRequest) =>
        await HandleWebhookRequest(webhookRequest, ScopeConstants.SubmissionCancelled);

    [Webhook("On submission analyzed", Description = "Triggered when a submission is analyzed.")]
    public async Task<WebhookResponse<SubmissionResponse>> OnSubmissionAnalyzed(WebhookRequest webhookRequest) =>
        await HandleWebhookRequest(webhookRequest, ScopeConstants.SubmissionAnalyzed);

    private async Task<WebhookResponse<SubmissionResponse>> HandleWebhookRequest(WebhookRequest webhookRequest, string expectedEventCode)
    {
        var payload = JsonConvert.DeserializeObject<SubmissionPayloadDto>(webhookRequest.Body.ToString()!);
        
        if (payload?.SubmissionId == null)
        {
            throw new Exception($"[GlobalLink] Got wrong webhook payload. Payload: {webhookRequest.Body}");
        }

        if (payload.EventCode != expectedEventCode)
        {
            return new WebhookResponse<SubmissionResponse>
            {
                Result = null,
                ReceivedWebhookRequestType = WebhookRequestType.Preflight
            };
        }

        try
        {
            var submission = await GetSubmissionAsync(payload.SubmissionId);
            return new WebhookResponse<SubmissionResponse>
            {
                Result = submission,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"[GlobalLink] Error while processing webhook: {ex.Message}. Most likely the submission is not found. Payload: {webhookRequest.Body}");
        }
    }

    private async Task<SubmissionResponse> GetSubmissionAsync(string submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}", Method.Get, Credentials);
        return await Client.ExecuteWithErrorHandling<SubmissionResponse>(apiRequest);
    }
}
