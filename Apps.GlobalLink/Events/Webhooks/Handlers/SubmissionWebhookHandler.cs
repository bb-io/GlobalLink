using System.Net;
using Apps.GlobalLink.Api;
using Apps.GlobalLink.Constants;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.GlobalLink.Events.Webhooks.Handlers;

public abstract class SubmissionWebhookHandler(
    InvocationContext invocationContext,
    string scope,
    string triggeringStatus,
    SubmissionRequest submissionRequest)
    : BaseInvocable(invocationContext), IWebhookEventHandler, IAfterSubscriptionWebhookEventHandler<SubmissionResponse>
{
    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
    {
        var credentials = authenticationCredentialsProvider.ToArray();
        var submissionId = GetSubmissionId();
        var payloadUrl = GetPayloadUrl(values);

        await SetMetadataAsync(credentials, submissionId, WebhookConstants.WebhookUrlMetadataKey, payloadUrl);
        await SetMetadataAsync(credentials, submissionId, WebhookConstants.WebhookScopeMetadataKey, scope);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
    {
        var credentials = authenticationCredentialsProvider.ToArray();
        var submissionId = GetSubmissionId();

        try
        {
            await DeleteMetadataAsync(credentials, submissionId, WebhookConstants.WebhookScopeMetadataKey);
            await DeleteMetadataAsync(credentials, submissionId, WebhookConstants.WebhookUrlMetadataKey);
        }
        catch (Exception exception)
        {
            var payloadUrl = values.TryGetValue(WebhookConstants.PayloadUrlKey, out var value) ? value : "N/A";
            InvocationContext.Logger?.LogError(
                $"[GlobalLinkWebhookHandler] Failed to unsubscribe from webhook ({scope}) for submission " +
                $"{submissionId}: {exception.Message}; Payload URL: {payloadUrl}", []);
            throw;
        }
    }

    public async Task<AfterSubscriptionEventResponse<SubmissionResponse>> OnWebhookSubscribedAsync()
    {
        var submissionId = GetSubmissionId();
        var submission = await GetSubmissionAsync(InvocationContext.AuthenticationCredentialsProviders.ToArray(), submissionId);

        if (!string.Equals(submission.Status, triggeringStatus, StringComparison.OrdinalIgnoreCase))
        {
            return null!;
        }

        return new AfterSubscriptionEventResponse<SubmissionResponse>
        {
            Result = submission
        };
    }

    private static async Task<SubmissionResponse> GetSubmissionAsync(
        AuthenticationCredentialsProvider[] credentials, string submissionId)
    {
        var client = new ApiClient(credentials);
        var request = new ApiRequest($"/rest/v0/submissions/{submissionId}", Method.Get, credentials);
        return await client.ExecuteWithErrorHandling<SubmissionResponse>(request);
    }

    private static async Task SetMetadataAsync(AuthenticationCredentialsProvider[] credentials, string submissionId,
        string key, string value)
    {
        var client = new ApiClient(credentials);
        var request = new ApiRequest($"/rest/v0/submissions/{submissionId}/metadata", Method.Post, credentials)
            .AddJsonBody(JsonConvert.SerializeObject(new { key, value }));

        await client.ExecuteWithErrorHandling(request);
    }

    private static async Task DeleteMetadataAsync(AuthenticationCredentialsProvider[] credentials, string submissionId,
        string key)
    {
        var client = new ApiClient(credentials);
        var request = new ApiRequest($"/rest/v0/submissions/{submissionId}/metadata/{key}", Method.Delete, credentials);
        var response = await client.ExecuteAsync(request);
        
        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        throw new PluginApplicationException(
            $"Failed to remove '{key}' from submission {submissionId}. Status code: {response.StatusCode}. " +
            $"Message: {response.Content ?? response.ErrorMessage}");
    }

    private string GetSubmissionId()
    {
        if (string.IsNullOrWhiteSpace(submissionRequest.SubmissionId))
        {
            throw new PluginMisconfigurationException(
                "Submission ID is not provided. Please specify the submission you want to subscribe to.");
        }

        return submissionRequest.SubmissionId;
    }

    private static string GetPayloadUrl(Dictionary<string, string> values)
    {
        if (!values.TryGetValue(WebhookConstants.PayloadUrlKey, out var payloadUrl) ||
            string.IsNullOrWhiteSpace(payloadUrl))
        {
            throw new PluginApplicationException(
                "Blackbird did not provide a payload URL for this subscription. Please contact blackbird support.");
        }

        return payloadUrl;
    }
}
