using Apps.GlobalLink.Api;
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
    [Webhook("On submission callback received", Description = "Triggered when a submission callback is received. This webhook may not be received immediately after the callback and could be delayed.")]
    public async Task<WebhookResponse<SubmissionResponse>> OnSubmissionCallbackReceived(WebhookRequest webhookRequest, 
        [WebhookParameter] SubmissionCallbackReceivedRequest callbackReceivedRequest) =>
        await HandleWebhookRequest(webhookRequest, callbackReceivedRequest.WebhookScopes);

    private async Task<WebhookResponse<SubmissionResponse>> HandleWebhookRequest(WebhookRequest webhookRequest, IEnumerable<string> expectedEventCode)
    {
        var payload = JsonConvert.DeserializeObject<SubmissionPayloadDto>(webhookRequest.Body.ToString()!);
        
        if (payload?.SubmissionId == null)
        {
            throw new Exception($"[GlobalLink] Got wrong webhook payload. Payload: {webhookRequest.Body}");
        }

        if (!expectedEventCode.Contains(payload.EventCode))
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
