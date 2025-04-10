using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Dtos;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Apps.GlobalLink.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.GlobalLink.Actions;

[ActionList]
public class SubmissionActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Search submissions", Description = "Searches for submissions based on various criteria such as due date, date started, status, and owner.")]
    public async Task<SearchSubmissionResponse> SearchSubmissionsAsync([ActionParameter] SearchSubmissionsRequest request)
    {
        var apiRequest = new ApiRequest("/rest/v0/submissions", Method.Get, Credentials)
            .AddQueryParameter("getBatchInfos", "true");

        if (request.DueDateFrom.HasValue)
        {
            apiRequest.AddQueryParameter("dueDateFrom",
                UnixTimestampConverter.ToUnixTimestamp(request.DueDateFrom.Value).ToString());
        }

        if (request.DueDateTo.HasValue)
        {
            apiRequest.AddQueryParameter("dueDateTo",
                UnixTimestampConverter.ToUnixTimestamp(request.DueDateTo.Value).ToString());
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            apiRequest.AddQueryParameter("statuses", request.Status);
        }

        var submissions = await Client.PaginateAsync<SubmissionResponse>(apiRequest);
        var result = submissions;

        if (!string.IsNullOrEmpty(request.OwnerId))
        {
            result = submissions
                .Where(s => s.Owners?.Any(o => string.Equals(o.UserId, request.OwnerId, StringComparison.OrdinalIgnoreCase)) == true)
                .ToList();
        }

        if(request.ExcludeSubmissionsWithOwnersAssigned == true)
        {
            result = result
                .Where(s => s.Owners == null || s.Owners.Count == 0)
                .ToList();
        }

        if(request.DateStartedFrom.HasValue)
        {
            result = result
                .Where(s => s.DateStarted >= request.DateStartedFrom.Value)
                .ToList();
        }

        if(request.DateStartedTo.HasValue)
        {
            result = result
                .Where(s => s.DateStarted <= request.DateStartedTo.Value)
                .ToList();
        }

        return new SearchSubmissionResponse(result);
    }

    [Action("Get submission", Description = "Retrieves a submission by its ID.")]
    public async Task<SubmissionResponse> GetSubmissionAsync([ActionParameter] SubmissionRequest submissionRequest)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionRequest.SubmissionId}", Method.Get, Credentials);
        var submission = await Client.ExecuteWithErrorHandling<SubmissionResponse>(apiRequest);
        return submission;
    }

    [Action("Create submission", Description = "Creates a new submission based on the provided parameters.")]
    public async Task<SubmissionResponse> CreateSubmissionAsync([ActionParameter] CreateSubmissionRequest request)
    {
        request.ThrowIfInvalid();

        var bodyDictionary = CreateSubmissionBody(request);
        var apiRequest = new ApiRequest("/rest/v0/submissions/create", Method.Post, Credentials)
            .AddJsonBody(bodyDictionary);

        var submissionIdDto = await Client.ExecuteWithErrorHandling<SubmissionIdDto>(apiRequest);
        await SaveSubmissionAsync(submissionIdDto.SubmissionId, false);

        return await GetSubmissionAsync(new SubmissionRequest
        {
            SubmissionId = submissionIdDto.SubmissionId
        });
    }

    [Action("Start submission", Description = "First analyzes and then starts a submission.")]
    public async Task<StartSubmissionResponse> StartSubmissionAsync([ActionParameter] SubmissionRequest submissionId)
    {
        await AnalyzeSubmissionAsync(submissionId.SubmissionId);
        await PollUntilProcessIsNotFinishedAsync(submissionId.SubmissionId);

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId.SubmissionId}/start", Method.Post, Credentials);
        var startSubmissionDto = await Client.ExecuteWithErrorHandling<StartSubmissionDto>(apiRequest);
        var response = new StartSubmissionResponse(startSubmissionDto);
        return response;
    }

    [Action("Claim submission", Description = "Claims a submission by its ID.")]
    public async Task ClaimSubmissionAsync([ActionParameter] ClaimSubmissionRequest claimSubmissionRequest)
    {
        var submissionRequest = new ApiRequest($"/rest/v0/submissions/{claimSubmissionRequest.SubmissionId}", Method.Get, Credentials);
        var response = await Client.ExecuteWithErrorHandling<SubmissionResponse>(submissionRequest);

        var requestBody = new[]
        {
            new
            {
                submissionId = claimSubmissionRequest.SubmissionId,
                phaseName = claimSubmissionRequest.PhaseName,
                languages = response.TargetLanguages
            }
        };

        var apiRequest = new ApiRequest($"/rest/v0/submissions/claim", Method.Post, Credentials)
            .AddJsonBody(requestBody);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }

    [Action("Cancel submission", Description = "Cancels a submission by its ID.")]
    public async Task CancelSubmissionAsync([ActionParameter] CancelSubmissionRequest submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/cancel/{submissionId.SubmissionId}", Method.Post, Credentials);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }

    private async Task SaveSubmissionAsync(string submissionId, bool autoStart)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/save", Method.Post, Credentials)
            .AddJsonBody(new
            {
                autoStart
            });

        await Client.ExecuteWithErrorHandling(apiRequest);
    }
    private async Task PollUntilProcessIsNotFinishedAsync(string submissionId)
    {
        const int MaxRetries = 30;
        const int DelayMilliseconds = 5000;
        var retries = 0;
        var lastMessage = string.Empty;

        while (retries < MaxRetries)
        {
            var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/status", Method.Get, Credentials);
            var statusResponse = await Client.ExecuteWithErrorHandling<SubmissionStatusResponse>(apiRequest);

            if (statusResponse.Status != "PROCESSING")
            {
                return;
            }

            retries++;
            lastMessage = statusResponse.Message;
            await Task.Delay(DelayMilliseconds);
        }

        throw new PluginApplicationException(
            $"Process timeout after {MaxRetries} attempts. Last message: {lastMessage}. Please contact blackbird support.");
    }

    private async Task AnalyzeSubmissionAsync(string submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/analyze", Method.Post, Credentials);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }

    private Dictionary<string, object> CreateSubmissionBody(CreateSubmissionRequest request)
    {
        var bodyDictionary = new Dictionary<string, object>
        {
            ["name"] = request.Name,
            ["dueDate"] = UnixTimestampConverter.ToUnixTimestamp(request.DueDate),
            ["projectId"] = request.ProjectId,
            ["sourceLanguage"] = request.SourceLanguage,
            ["instructions"] = request.Instructions ?? string.Empty,
            ["background"] = request.Background ?? string.Empty,
            ["claimScope"] = request.ClaimScope ?? "LANGUAGE",
            ["batchInfos"] = new[] { CreateBatchInfo(request) }
        };

        var metadata = new List<KeyValueDto>();
        if (metadata.Any())
        {
            bodyDictionary["metadata"] = metadata;
        }

        return bodyDictionary;
    }

    private static object CreateBatchInfo(CreateSubmissionRequest request)
    {
        var targetLanguageInfos = request.TargetLanguages
            .Select(lang => new { targetLanguage = lang })
            .ToList();

        return new
        {
            workflowId = string.IsNullOrEmpty(request.WorkflowId) ? null : request.WorkflowId,
            targetLanguageInfos,
            targetFormat = request.TargetFormat ?? "TXLF",
            name = request.BatchName ?? "Batch1"
        };
    }
}
