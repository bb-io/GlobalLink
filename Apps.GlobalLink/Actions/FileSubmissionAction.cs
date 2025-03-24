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

    [Action("Upload target file", Description = "Uploads a target file to a submission and waits for the process to finish successfully. Important: The file name must not be modified in any way. It is encoded in the filename and is essential for the upload process to work correctly.")]
    public async Task UploadTargetFileAsync([ActionParameter] UploadTargetFileRequest request)
    {
        var stream = await fileManagementClient.DownloadAsync(request.File);
        var bytes = await stream.GetByteData();

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{request.SubmissionId}/upload/translatable", Method.Post, Credentials)
            .AddFile("file", bytes, request.File.Name);

        var process = await Client.ExecuteWithErrorHandling<ProcessDto>(apiRequest);
        await PollForTargetFileUploadingCompletionAsync(request.SubmissionId, process.ProcessId);

        var phase = await GetPhaseForFileAsync(request.SubmissionId, request.File.Name);
        await CompletePhaseAsync(request.SubmissionId, phase);
    }

    [Action("Download source files", Description = "Downloads a source file from a submission.")]
    public async Task<DownloadSourceFilesResponse> DownloadSourceFilesAsync([ActionParameter] DownloadSourceFilesRequest request)
    {
        var targets = await GetTargetsAsync(request.SubmissionId, "IN_PROCESS");
        var allFiles = await ProcessTargetsAsync(request.SubmissionId, targets, request.PhaseName);
        return new DownloadSourceFilesResponse
        {
            SourceFiles = allFiles
        };
    }

    [Action("Download target files", Description = "Downloads a translated file from a submission.")]
    public async Task<DownloadTargetFilesResponse> DownloadTargetFilesAsync([ActionParameter] DownloadTargetFilesRequest request)
    {
        var targets = await GetTargetsAsync(request.SubmissionId, "PROCESSED");
        var allFiles = await ProcessTargetsAsync(request.SubmissionId, targets);
        return new DownloadTargetFilesResponse
        {
            TargetFiles = allFiles
        };
    }

    private async Task CompletePhaseAsync(string submissionId, PhaseResponse phaseResponse)
    {
        var transition = phaseResponse.Transitions.FirstOrDefault();
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/phases/{phaseResponse.CurrentPhase}/complete", Method.Post, Credentials)
            .AddJsonBody(new
             { 
                targetIds = new List<long> { long.Parse(phaseResponse.TargetId) },
                transition = transition?.Name ?? null,
             });

        var completeDto = await Client.ExecuteWithErrorHandling<CompletePhaseDto>(apiRequest);
        if(completeDto.FailedTargets.Length != 0)
        {
            var failedTargets = string.Join(", ", completeDto.FailedTargets.Select(x => x));
            throw new PluginApplicationException($"Failed to complete phase for target IDs: {failedTargets}. Please ask blackbird support for further investigation and explanation.");
        }
    }

    private async Task<PhaseResponse> GetPhaseForFileAsync(string submissionId, string fileName) 
    {
        var request = new ApiRequest($"/rest/v0/submissions/{submissionId}/phases?targetids&documentId", Method.Get, Credentials);
        var phases = await Client.PaginateAsync<PhaseResponse>(request);
        var phase = phases.FirstOrDefault(x => x.TargetFileName == fileName);
        if (phase == null)
        {
            var allFileNames = string.Join(", ", phases.Select(x => x.TargetFileName));
            throw new PluginApplicationException($"No target file found for submission ID {submissionId} with file name {fileName}. Please check that you didn't modified the original file name you downloaded from this app. Here are the available file names: {allFileNames}.");
        }

        return phase;
    }

    private async Task PollForTargetFileUploadingCompletionAsync(string submissionId, string processId)
    {
        const int MaxRetries = 30;
        const int DelayMilliseconds = 5000;
        var retries = 0;

        while (retries < MaxRetries)
        {
            var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/upload/translatable/{processId}", Method.Get, Credentials);
            var processDto = await Client.ExecuteWithErrorHandling<TranslatableUploadProcessDto>(apiRequest);

            if (processDto.HasUploadSucceeded)
            {
                return;
            }
            else if(processDto.HasProcessingFailed)
            {
                throw new PluginApplicationException($"Upload process failed with message: {processDto.GetErrorMessage()}");
            }

            retries++;
            await Task.Delay(DelayMilliseconds);
        }

        throw new PluginApplicationException(
            $"Upload process timeout after {MaxRetries} attempts. Please contact blackbird support for futher investigation and explanation.");
    }

    private async Task<List<TargetResponse>> GetTargetsAsync(string submissionId, string targetStatus)
    {
        const int MaxRetries = 3;
        var baseDelayMilliseconds = 5000;
        
        for (int retry = 0; retry < MaxRetries; retry++)
        {
            var apiRequest = new ApiRequest($"/rest/v0/targets", Method.Get, Credentials)
                .AddQueryParameter("targetStatus", targetStatus)
                .AddQueryParameter("submissionIds", submissionId);

            var targets = await Client.ExecuteWithErrorHandling<List<TargetResponse>>(apiRequest);
            if (targets.Count > 0)
            {
                return targets;
            }
            
            if (retry < MaxRetries - 1)
            {
                var delayTime = baseDelayMilliseconds * (retry + 1);
                await Task.Delay(delayTime);
            }
        }
        
        throw new PluginApplicationException($"No target files found for submission ID {submissionId} that are in {targetStatus} status after {MaxRetries} retries.");
    }

    private async Task<List<DownloadFileGroupResponse>> ProcessTargetsAsync(string submissionId, List<TargetResponse> targets, string? phaseName = null)
    {
        var allFiles = new List<DownloadFileGroupResponse>();
        foreach (var target in targets)
        {
            var downloadProcessDto = phaseName == null
                ? await InitiateDeliverableDownloadAsync(submissionId, target.TargetId)
                : await InitiateTranslatableDownloadAsync(submissionId, target.TargetId, phaseName);
            
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

    private async Task<DownloadProcessDto> InitiateDeliverableDownloadAsync(string submissionId, string targetId)
    {
        var downloadRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
            .AddQueryParameter("deliverableTargetIds", targetId)
            .AddQueryParameter("includeManifest", false);

        return await Client.ExecuteWithErrorHandling<DownloadProcessDto>(downloadRequest);
    }

    private async Task<DownloadProcessDto> InitiateTranslatableDownloadAsync(string submissionId, string targetId, string phaseName)
    {
        var downloadRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
            .AddQueryParameter("translatableFiles", true)
            .AddQueryParameter("phaseName", phaseName)
            .AddQueryParameter("targetIds", targetId)
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
