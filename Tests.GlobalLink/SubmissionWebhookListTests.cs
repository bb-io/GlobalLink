using Apps.GlobalLink.Events;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.GlobalLink.Base;

namespace Tests.GlobalLink;

[TestClass]
public class SubmissionWebhookListTests : TestBase
{
    private readonly SubmissionWebhookList _webhookList;
    
    public SubmissionWebhookListTests()
    {
        _webhookList = new SubmissionWebhookList(InvocationContext);
    }

    [TestMethod]
    public async Task OnSubmissionCompleted_WithValidPayload_ShouldReturnSubmissionResponse()
    {
        // Act
        var response = await ExecuteWebhookTest("submission.completed",
            _webhookList.OnSubmissionCompleted);

        // Assert
        AssertWebhookResponse(response, "17896");
    }

    [TestMethod]
    public async Task OnSubmissionCancelled_WithValidPayload_ShouldReturnSubmissionResponse()
    {
        // Act
        var response = await ExecuteWebhookTest("submission.cancelled",
            _webhookList.OnSubmissionCancelled);

        // Assert
        AssertWebhookResponse(response, "17896");
    }

    [TestMethod]
    public async Task OnSubmissionAnalyzed_WithValidPayload_ShouldReturnSubmissionResponse()
    {
        // Act
        var response = await ExecuteWebhookTest("submission.analyzed",
            _webhookList.OnSubmissionAnalyzed);

        // Assert
        AssertWebhookResponse(response, "17896");
    }

    private async Task<WebhookResponse<SubmissionResponse>> ExecuteWebhookTest(
        string eventCode, 
        Func<WebhookRequest, Task<WebhookResponse<SubmissionResponse>>> webhookAction)
    {
        // Arrange
        string webhookPayload = @"{
            ""documentIds"":[78333,78335],
            ""targetIds"":[],
            ""eventTime"":1742811090000,
            ""pdUrl"":""https://xxxxx/PD"",
            ""submissionId"":17896,
            ""projectName"":""Blackbird_Test"",
            ""submissionName"":""Test Submission With Webhooks 1"",
            ""eventCode"":""" + eventCode + @"""
        }";

        var webhookRequest = new WebhookRequest
        {
            Body = webhookPayload,
            Headers = new Dictionary<string, string>(),
            HttpMethod = HttpMethod.Post,
            Url = "https://example.com/webhook",
            QueryParameters = new Dictionary<string, string>()
        };

        // Execute the webhook action
        return await webhookAction(webhookRequest);
    }

    private void AssertWebhookResponse(WebhookResponse<SubmissionResponse> response, string expectedSubmissionId)
    {
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual(expectedSubmissionId, response.Result.SubmissionId);
        
        // Additional assertions based on the expected submission properties
        Console.WriteLine($"Webhook response: {JsonConvert.SerializeObject(response.Result, Formatting.Indented)}");
    }
}
