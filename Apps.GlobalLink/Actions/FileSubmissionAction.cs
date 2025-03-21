using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Dtos;
using Apps.GlobalLink.Models.Requests.Submissions;
using Apps.GlobalLink.Models.Responses.Submissions;
using Apps.GlobalLink.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.GlobalLink.Actions;

[ActionList]
public class FileSubmissionAction(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    private readonly IZipFileService zipFileService = new ZipFileService(fileManagementClient);

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

    [Action("Download target files", Description = "Downloads a translated file from a submission.")]
    public async Task<DownloadTargetFilesResponse> DownloadTargetFilesAsync([ActionParameter] DownloadTargetFilesRequest request)
    {
        try
        {
            var targets = await GetProcessedTargetsAsync(request.SubmissionId);
            var allFiles = await ProcessTargetsAsync(request.SubmissionId, targets);
            return new DownloadTargetFilesResponse
            {
                TargetFiles = allFiles
            };
        }
        catch(Exception ex)
        {
            throw new PluginApplicationException($"{ex.Message}; {ex.StackTrace}");
        }
    }

    private async Task<List<TargetResponse>> GetProcessedTargetsAsync(string submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/targets", Method.Get, Credentials)
            .AddQueryParameter("targetStatus", "PROCESSED")
            .AddQueryParameter("submissionIds", submissionId);
        
        var targets = await Client.ExecuteWithErrorHandling<List<TargetResponse>>(apiRequest);
        if (targets.Count == 0)
        {
            throw new PluginApplicationException($"No target files found for submission ID {submissionId} that are in PROCESSED status.");
        }

        return targets;
    }

    private async Task<List<DownloadTargetFileGroupResponse>> ProcessTargetsAsync(string submissionId, List<TargetResponse> targets)
    {
        var allFiles = new List<DownloadTargetFileGroupResponse>();
        foreach (var target in targets)
        {
            var downloadProcessDto = await InitiateDownloadAsync(submissionId, target.TargetId);

            if (!downloadProcessDto.ProcessingFinished)
            {
                await PollDownloadProcessCompletionAsync(submissionId, downloadProcessDto.DownloadId);
            }

            var downloadedFiles = await DownloadAndExtractFilesAsync(downloadProcessDto.DownloadId);
            downloadedFiles.ForEach(file => allFiles.Add(new()
            {
                SourceLanguage = target.SourceLanguage,
                TargetLanguage = target.TargetLanguage,
                File = file
            }));
        }

        return allFiles;
    }

    private async Task<DownloadProcessDto> InitiateDownloadAsync(string submissionId, string targetId)
    {
        var downloadRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
            .AddQueryParameter("deliverableTargetIds", targetId)
            .AddQueryParameter("includeManifest", false);

        return await Client.ExecuteWithErrorHandling<DownloadProcessDto>(downloadRequest);
    }

    private async Task<List<FileReference>> DownloadAndExtractFilesAsync(string downloadId)
    {
        var downloadZipRequest = new ApiRequest($"/rest/v0/submissions/download/{downloadId}", Method.Get, Credentials);
        var downloadZipResponse = await Client.ExecuteWithErrorHandling(downloadZipRequest);

        var contentDisposition = downloadZipResponse.Headers?.FirstOrDefault(x => x.Name == "Content-Disposition")?.Value?.ToString();
        var zipBytes = downloadZipResponse.RawBytes!;
        var memoryStream = new MemoryStream(zipBytes);
        memoryStream.Position = 0;

        return await zipFileService.ExtractZipFilesAsync(memoryStream);
    }

    private async Task PollDownloadProcessCompletionAsync(string submissionId, string downloadId)
    {
        const int MaxRetries = 30;
        const int DelayMilliseconds = 5000;
        var retries = 0;
        var lastMessage = string.Empty;

        while (retries < MaxRetries)
        {
            var downloadRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
                .AddQueryParameter("downloadId", downloadId);

            var downloadProcessDto = await Client.ExecuteWithErrorHandling<DownloadProcessDto>(downloadRequest);

            if (downloadProcessDto.ProcessingFinished)
            {
                return;
            }

            retries++;
            lastMessage = downloadProcessDto.Message;
            await Task.Delay(DelayMilliseconds);
        }

        throw new PluginApplicationException(
            $"Download process timeout after {MaxRetries} attempts. Last message: {lastMessage}. Please contact blackbird support.");
    }
}
