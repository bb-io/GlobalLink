using Apps.GlobalLink.Api;
using Apps.GlobalLink.Events.Models;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.GlobalLink.Events;

[PollingEventList]
public class SubmissionPollingList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    private const string InProcessStatus = "IN_PROCESS";
    private const string ProcessedStatus = "PROCESSED";

    [PollingEvent("On submissions created", Description = "Polling event that periodically checks for new submissions. If new submissions are found, the event is triggered.")]
    public Task<PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>> OnSubmissionsCreatedAsync(
        PollingEventRequest<SubmissionMemory> request,
        [PollingEventParameter] SubmissionOptionalRequest submissionOptionalRequest) => HandlePollingEventAsync(request, submissionOptionalRequest, InProcessStatus);

    [PollingEvent("On submission completed", Description = "Polling event that periodically checks for completed submissions. If completed submissions are found, the event is triggered.")]
    public Task<PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>> OnSubmissionCompletedAsync(
        PollingEventRequest<SubmissionMemory> request,
        [PollingEventParameter] SubmissionOptionalRequest submissionOptionalRequest) => HandlePollingEventAsync(request, submissionOptionalRequest, ProcessedStatus);

    private async Task<PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>> HandlePollingEventAsync(
        PollingEventRequest<SubmissionMemory> request,
        SubmissionOptionalRequest submissionOptionalRequest,
        string status)
    {
        if (!string.IsNullOrEmpty(submissionOptionalRequest.SubmissionId))
        {
            return await PollForSubmissionAsync(request, submissionOptionalRequest, status);
        }

        return await PollForSubmissionsAsync(request, status);
    }

    private async Task<PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>> PollForSubmissionAsync(
        PollingEventRequest<SubmissionMemory> request,
        SubmissionOptionalRequest submissionOptionalRequest,
        string status)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionOptionalRequest.SubmissionId}", Method.Get, Credentials);
        var submission = await Client.ExecuteWithErrorHandling<SubmissionResponse>(apiRequest);

        var oldSubmissionIds = request.Memory?.SubmissionIds ?? [];
        bool flyBird = !oldSubmissionIds.Contains(submission.SubmissionId) && submission.Status == status;

        var submissionMemory = request.Memory ?? new SubmissionMemory
        {
            LastPollingTime = DateTime.UtcNow,
            SubmissionIds = [.. oldSubmissionIds]
        };

        if (flyBird)
        {
            submissionMemory.SubmissionIds.Add(submission.SubmissionId);
        }

        return new PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>
        {
            Result = new([submission]),
            Memory = submissionMemory,
            FlyBird = flyBird,
        };
    }

    private async Task<PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>> PollForSubmissionsAsync(
        PollingEventRequest<SubmissionMemory> request,
        string status)
    {
        var apiRequest = new ApiRequest("/rest/v0/submissions", Method.Get, Credentials)
            .AddQueryParameter("statuses", status);

        var submissions = await Client.PaginateAsync<SubmissionResponse>(apiRequest);

        if (request.Memory == null)
        {
            return new PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>
            {
                Result = new([]),
                Memory = new SubmissionMemory
                {
                    LastPollingTime = DateTime.UtcNow,
                    SubmissionIds = submissions.Select(s => s.SubmissionId).ToList()
                },
                FlyBird = false
            };
        }

        var oldSubmissionIds = request.Memory?.SubmissionIds ?? [];
        var newSubmissions = submissions
            .Where(s => !oldSubmissionIds.Contains(s.SubmissionId))
            .ToList();

        var submissionMemory = request.Memory ?? new SubmissionMemory
        {
            LastPollingTime = DateTime.UtcNow,
            SubmissionIds = [.. oldSubmissionIds]
        };

        foreach (var submission in newSubmissions)
        {
            submissionMemory.SubmissionIds.Add(submission.SubmissionId);
        }

        var result = new SearchSubmissionResponse(newSubmissions);
        var flyBird = newSubmissions.Any();
        return new PollingEventResponse<SubmissionMemory, SearchSubmissionResponse>
        {
            Result = result,
            Memory = submissionMemory,
            FlyBird = flyBird
        };
    }
}
