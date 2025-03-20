using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Dtos;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Apps.GlobalLink.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.GlobalLink.Actions;

[ActionList]
public class SubmissionActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Get submission", Description = "Retrieves a submission by its ID.")]
    public async Task<SubmissionResponse> GetSubmissionAsync([ActionParameter] SubmissionRequest submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId.SubmissionId}", Method.Get, Credentials);
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

    [Action("Upload source file", Description = "Uploads a source file to a submission.")]
    public async Task<UploadSourceFileResponse> UploadSourceFileAsync([ActionParameter] UploadSourceFileRequest request)
    {
        var stream = await fileManagementClient.DownloadAsync(request.File);
        var bytes = await stream.GetByteData();

        // TODO: handle fileFormatName optional parameter
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{request.SubmissionId}/upload/source", Method.Post, Credentials)
            .AddFile("file", bytes, request.File.Name)
            .AddParameter("batchName", request.BatchName ?? "Batch1");

        return await Client.ExecuteWithErrorHandling<UploadSourceFileResponse>(apiRequest);
    }

    [Action("Upload reference file", Description = "Uploads a reference file to a submission ob submission level.")]
    public async Task UploadReferenceFileAsync([ActionParameter] UploadReferenceFileRequest request)
    {
        var stream = await fileManagementClient.DownloadAsync(request.File);
        var bytes = await stream.GetByteData();

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{request.SubmissionId}/upload/reference", Method.Post, Credentials)
            .AddFile("file", bytes, request.File.Name)
            .AddParameter("submissionLevel", true);
        await Client.ExecuteWithErrorHandling(apiRequest);
    }

    [Action("Start submission", Description = "First analyzes and then starts a submission.")]
    public async Task<StartSubmissionResponse> StartSubmissionAsync([ActionParameter] SubmissionRequest submissionId)
    {
        await AnalyzeSubmissionAsync(submissionId.SubmissionId);
        await PullUntilProcessIsNotFinishedAsync(submissionId.SubmissionId);

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId.SubmissionId}/start", Method.Post, Credentials);
        var startSubmissionDto = await Client.ExecuteWithErrorHandling<StartSubmissionDto>(apiRequest);
        var response = new StartSubmissionResponse(startSubmissionDto);
        return response;
    }

    [Action("Claim submission", Description = "Claims a submission by its ID.")]
    public async Task ClaimSubmissionAsync([ActionParameter] ClaimSubmissionRequest submissionId)
    {
        var requestBody = new[]
        {
            new 
            {
                submissionId = submissionId.SubmissionId,
                phaseName = submissionId.PhaseName,
                languages = submissionId.TargetLanguages
            }
        };

        var apiRequest = new ApiRequest($"/rest/v0/submissions/claim", Method.Post, Credentials)
            .AddJsonBody(requestBody);
            
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

    private async Task PullUntilProcessIsNotFinishedAsync(string submissionId)
    {
        const int MaxRetries = 10;
        var retries = 0;
        var lastStatus = string.Empty;
        var lastMessage = string.Empty;

        while (true)
        {
            if (retries >= MaxRetries)
            {
                throw new PluginApplicationException($"It took too long time to process the submission. Last status: {lastStatus}. Last message: {lastMessage}. Please, contact blackbird support for further assistance.");
            }

            var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/status", Method.Get, Credentials);
            var statusResponse = await Client.ExecuteWithErrorHandling<SubmissionStatusResponse>(apiRequest);

            if (statusResponse.Status != "PROCESSING")
            {
                break;
            }

            retries += 1;
            lastStatus = statusResponse.Status;
            lastMessage = statusResponse.Message;
            await Task.Delay(5000);
        }
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
            ["claimScope"] = request.ClaimScope!,
            ["batchInfos"] = new[] { CreateBatchInfo(request) }
        };

        var metadata = CreateMetadata(request);
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
            targetFormat = request.TargetFormat,
            name = request.BatchName
        };
    }

    private static List<KeyValueDto> CreateMetadata(CreateSubmissionRequest request)
    {
        var metadata = new List<KeyValueDto>();
        
        if (string.IsNullOrEmpty(request.WebhookUrl)) 
            return metadata;
            
        metadata.Add(new KeyValueDto
        {
            Key = "_webhookURL",
            Value = request.WebhookUrl
        });

        if (!string.IsNullOrEmpty(request.WebhookScope))
        {
            metadata.Add(new KeyValueDto
            {
                Key = "_webhookScope",
                Value = request.WebhookScope
            });
        }

        return metadata;
    }
}
