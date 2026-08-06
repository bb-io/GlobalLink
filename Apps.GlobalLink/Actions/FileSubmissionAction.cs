using Apps.GlobalLink.Api;
using Apps.GlobalLink.Models.Dtos;
using Apps.GlobalLink.Models.Enums;
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

[ActionList("File submission")]
public class FileSubmissionAction(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    private const string DefaultBatchName = "Batch1";
    private const string NonParsableTargetFormat = "NON_PARSABLE";

    private readonly IZipFileService zipFileService = new ZipFileService(fileManagementClient);
    private readonly IFileFormatResolver fileFormatResolver = new FileFormatResolver(invocationContext.AuthenticationCredentialsProviders);

    [Action("Upload source file", Description = "Uploads a source file to a submission.")]
    public async Task<UploadSourceFileResponse> UploadSourceFileAsync([ActionParameter] UploadSourceFileRequest request)
    {
        var stream = await fileManagementClient.DownloadAsync(request.File);
        var bytes = await stream.GetByteData();

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{request.SubmissionId}/upload/source", Method.Post, Credentials)
            .AddFile("file", bytes, request.File.Name)
            .AddParameter("batchName", request.BatchName ?? DefaultBatchName);

        var fileFormatName = await ResolveFileFormatNameAsync(request);
        if (!string.IsNullOrWhiteSpace(fileFormatName))
        {
            // GlobalLink requires this parameter to be omitted entirely, not sent empty, when there is no parser to apply.
            apiRequest.AddParameter("fileFormatName", fileFormatName);
        }

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

    [Action("Reopen target file", Description = "Uploads an updated version of an already completed target file and re-completes it via the submission's linked reopen workflow. Use this when issues are found after final delivery and the target needs to be corrected. Note: this does not move the target backward in the workflow, it only re-uploads and re-completes it.")]
    public async Task<ReopenTargetFileResponse> ReopenTargetFileAsync([ActionParameter] ReopenTargetFileRequest request)
    {
        var submissionActions = new SubmissionActions(InvocationContext);
        var submission = await submissionActions.GetSubmissionAsync(new() { SubmissionId = request.SubmissionId });

        var batch = submission.BatchInfos.FirstOrDefault();
        if (batch == null)
        {
            throw new PluginApplicationException($"Submission ID {request.SubmissionId} has no batches, so its workflow could not be determined.");
        }

        var reopenWorkflowId = await ResolveReopenWorkflowIdAsync(submission.ProjectId, batch.WorkflowId.ToString(), request.ReopenWorkflowId);

        var stream = await fileManagementClient.DownloadAsync(request.File);
        var bytes = await stream.GetByteData();

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{request.SubmissionId}/upload/reopen", Method.Post, Credentials)
            .AddFile("file", bytes, request.File.Name)
            .AddParameter("reopenWorkflowId", reopenWorkflowId);

        if (request.ExtractArchive == true)
        {
            apiRequest.AddParameter("extractArchive", true);
        }

        var process = await Client.ExecuteWithErrorHandling<ProcessDto>(apiRequest);
        await PollForTargetFileUploadingCompletionAsync(request.SubmissionId, process.ProcessId);

        return new ReopenTargetFileResponse { ReopenWorkflowId = reopenWorkflowId };
    }

    [Action("Download source files", Description = "Downloads a source file from a submission.")]
    public async Task<DownloadSourceFilesResponse> DownloadSourceFilesAsync([ActionParameter] DownloadSourceFilesRequest request)
    {
        var submissionActions = new SubmissionActions(InvocationContext);
        var submission = await submissionActions.GetSubmissionAsync(new() { SubmissionId = request.SubmissionId });
        if (submission.Status == "PROCESSED")
        {
            throw new PluginMisconfigurationException("The submission is already processed. You cannot download source files from a processed submission. Please create a new submission to upload source files.");
        }

        var targets = await GetTargetsAsync(request.SubmissionId, ["IN_PROCESS"], FileRole.Source);
        if (targets.Count == 0)
        {
            return await DownloadSourceFilesWithoutTargets(request.SubmissionId, submission.SourceLanguage, submission.TargetLanguages);
        }

        var allFiles = await ProcessTargetsAsync(request.SubmissionId, targets, request.PhaseName);
        return new DownloadSourceFilesResponse
        {
            SourceFiles = allFiles
        };
    }

    [Action("Download target files", Description = "Downloads a translated file from a submission.")]
    public async Task<DownloadTargetFilesResponse> DownloadTargetFilesAsync([ActionParameter] DownloadTargetFilesRequest request)
    {
        var targets = await GetTargetsAsync(request.SubmissionId, ["PROCESSED", "DELIVERED"], FileRole.Target);
        var allFiles = await ProcessTargetsAsync(request.SubmissionId, targets);
        return new DownloadTargetFilesResponse
        {
            TargetFiles = allFiles
        };
    }

    [Action("Mark targets as delivered", Description = "Marks targets of a submission as delivered, changing their status from 'PROCESSED' to 'DELIVERED'. This flags the content as successfully returned to the client, and, for clients using polling, prevents delivered targets from appearing again on the next polling cycle. If no target IDs are specified, all targets currently in 'PROCESSED' status for the submission are marked as delivered.")]
    public async Task<MarkTargetsAsDeliveredResponse> MarkTargetsAsDeliveredAsync([ActionParameter] MarkTargetsAsDeliveredRequest request)
    {
        var targetIds = request.TargetIds?.Where(id => !string.IsNullOrWhiteSpace(id)).ToList() ?? [];
        if (targetIds.Count == 0)
        {
            targetIds = await GetProcessedTargetIdsAsync(request.SubmissionId);
        }

        if (targetIds.Count == 0)
        {
            throw new PluginApplicationException($"No targets in 'PROCESSED' status were found for submission ID {request.SubmissionId}.");
        }

        var apiRequest = new ApiRequest($"/rest/v0/submissions/{request.SubmissionId}/targets/delivered", Method.Post, Credentials)
            .AddJsonBody(new
            {
                targetIds = targetIds.Select(long.Parse)
            });

        var result = await Client.ExecuteWithErrorHandling<MarkTargetsAsDeliveredDto>(apiRequest);
        return new MarkTargetsAsDeliveredResponse
        {
            SuccessfullyDeliveredTargetIds = result.SuccessfullyDeliveredTargets.Select(x => x.ToString()),
            FailedToDeliverTargetIds = result.FailedToDeliverTargets.Select(x => x.ToString())
        };
    }

    [Action("Download reference files", Description = "Download submission-level and/or language-level reference files for a submission.")]
    public async Task<DownloadReferenceFilesResponse> DownloadReferenceFilesAsync([ActionParameter] DownloadReferenceFilesRequest request)
    {
        if (request.SubmissionLevel != true && (request.Languages == null || !request.Languages.Any()))
            throw new PluginMisconfigurationException("Specify at least one: submission-level references or language-level references.");

        var submissionActions = new SubmissionActions(InvocationContext);
        var submission = await submissionActions.GetSubmissionAsync(new() { SubmissionId = request.SubmissionId });

        var allFiles = new List<DownloadFileGroupResponse>();

        if (request.SubmissionLevel == true)
        {
            var proc = await InitiateReferenceDownload_SubmissionLevelAsync(request.SubmissionId);
            if (!proc.ProcessingFinished)
                await PollDownloadProcessCompletionAsync(request.SubmissionId, proc.DownloadId);

            var files = await DownloadAndExtractFilesAsync(proc.DownloadId);
            foreach (var targetLang in submission.TargetLanguages)
            {
                files.ForEach(f => allFiles.Add(new DownloadFileGroupResponse
                {
                    SourceLanguage = submission.SourceLanguage,
                    TargetLanguage = targetLang,
                    File = f
                }));
            }
        }

        var langs = request.Languages?.Where(l => !string.IsNullOrWhiteSpace(l)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (langs != null && langs.Count > 0)
        {
            foreach (var lang in langs)
            {
                var proc = await InitiateReferenceDownload_LanguageLevelAsync(request.SubmissionId, lang);
                if (!proc.ProcessingFinished)
                    await PollDownloadProcessCompletionAsync(request.SubmissionId, proc.DownloadId);

                var files = await DownloadAndExtractFilesAsync(proc.DownloadId);
                files.ForEach(f => allFiles.Add(new DownloadFileGroupResponse
                {
                    SourceLanguage = submission.SourceLanguage,
                    TargetLanguage = lang,
                    File = f
                }));
            }
        }

        return new DownloadReferenceFilesResponse { ReferenceFiles = allFiles };
    }

    private async Task<string?> ResolveFileFormatNameAsync(UploadSourceFileRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.FileFormatName))
        {
            return request.FileFormatName;
        }

        try
        {
            var submission = await GetSubmissionWithBatchInfosAsync(request.SubmissionId);

            var batchName = request.BatchName ?? DefaultBatchName;
            var batch = submission.BatchInfos.FirstOrDefault(x => string.Equals(x.Name, batchName, StringComparison.OrdinalIgnoreCase));
            if (string.Equals(batch?.TargetFormat, NonParsableTargetFormat, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return await fileFormatResolver.ResolveFileFormatNameAsync(submission.ProjectId, request.File);
        }
        catch (Exception)
        {
            // Detecting the parser is a best effort: when it fails, e.g. because the connection has no access to the
            // project file formats, we keep uploading without it and let GlobalLink apply its own default parser.
            return null;
        }
    }

    private async Task<SubmissionResponse> GetSubmissionWithBatchInfosAsync(string submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}", Method.Get, Credentials)
            .AddQueryParameter("getBatchInfos", true);

        return await Client.ExecuteWithErrorHandling<SubmissionResponse>(apiRequest);
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
        if (completeDto.FailedTargets.Length != 0)
        {
            var failedTargets = string.Join(", ", completeDto.FailedTargets.Select(x => x));
            throw new PluginApplicationException($"Failed to complete phase for target IDs: {failedTargets}. Please ask blackbird support for further investigation and explanation.");
        }
    }

    private async Task<DownloadProcessDto> InitiateReferenceDownload_SubmissionLevelAsync(string submissionId)
    {
        var req = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
            .AddQueryParameter("submissionReferenceFiles", true)
            .AddQueryParameter("includeManifest", false);

        return await Client.ExecuteWithErrorHandling<DownloadProcessDto>(req);
    }

    private async Task<DownloadProcessDto> InitiateReferenceDownload_LanguageLevelAsync(string submissionId, string language)
    {
        var req = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
            .AddQueryParameter("languageReferenceFiles", language)
            .AddQueryParameter("includeManifest", false);

        return await Client.ExecuteWithErrorHandling<DownloadProcessDto>(req);
    }

    private async Task<DownloadSourceFilesResponse> DownloadSourceFilesWithoutTargets(string submissionId, string sourceLanguage, List<string> targetLanguages)
    {
        var downloadSourceFilesRequest = new ApiRequest($"/rest/v0/submissions/{submissionId}/download", Method.Get, Credentials)
            .AddQueryParameter("sourceFiles", "true")
            .AddQueryParameter("includeManifest", "false");

        var downloadProcessDto = await Client.ExecuteWithErrorHandling<DownloadProcessDto>(downloadSourceFilesRequest);
        if (!downloadProcessDto.ProcessingFinished)
        {
            await PollDownloadProcessCompletionAsync(submissionId, downloadProcessDto.DownloadId);
        }

        var downloadedFiles = await DownloadAndExtractFilesAsync(downloadProcessDto.DownloadId);
        var sourceFiles = new List<DownloadFileGroupResponse>();
        foreach (var downloadedFile in downloadedFiles)
        {
            foreach (var targetLanguage in targetLanguages)
            {
                sourceFiles.Add(new DownloadFileGroupResponse
                {
                    SourceLanguage = sourceLanguage,
                    TargetLanguage = targetLanguage,
                    File = downloadedFile
                });
            }
        }

        return new DownloadSourceFilesResponse
        {
            SourceFiles = sourceFiles
        };
    }

    private async Task<PhaseResponse> GetPhaseForFileAsync(string submissionId, string fileName)
    {
        var request = new ApiRequest($"/rest/v0/submissions/{submissionId}/phases?targetids&documentId", Method.Get, Credentials);
        var phases = await Client.PaginateAsync<PhaseResponse>(request);

        var fileNameWithoutTxlf = fileName.EndsWith(".txlf", StringComparison.OrdinalIgnoreCase)
            ? fileName.Substring(0, fileName.Length - 5)
            : fileName;

        var phase = phases.FirstOrDefault(x =>
            string.Equals(x.TargetFileName, fileName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TargetFileName, fileNameWithoutTxlf, StringComparison.OrdinalIgnoreCase));

        if (phase == null)
        {
            var allFileNames = string.Join(", ", phases.Select(x => x.TargetFileName));
            throw new PluginApplicationException($"No target file found for submission ID {submissionId} with file name {fileName}. Please check that you didn't modify the original file name you downloaded from this app. Here are the available file names: {allFileNames}.");
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
            else if (processDto.HasProcessingFailed)
            {
                throw new PluginApplicationException($"Upload process failed with message: {processDto.GetErrorMessage()}");
            }

            retries++;
            await Task.Delay(DelayMilliseconds);
        }

        throw new PluginApplicationException(
            $"Upload process timeout after {MaxRetries} attempts. Please contact blackbird support for futher investigation and explanation.");
    }

    private async Task<List<TargetResponse>> GetTargetsAsync(string submissionId, List<string> targetStatuses, FileRole fileRole)
    {
        const int MaxRetries = 3;
        var baseDelayMilliseconds = 5000;

        for (int retry = 0; retry < MaxRetries; retry++)
        {
            var statusDivider = string.Join(",", targetStatuses);
            var apiRequest = new ApiRequest($"/rest/v0/targets", Method.Get, Credentials)
                .AddQueryParameter("targetStatus", statusDivider)
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

        if (fileRole == FileRole.Source)
        {
            return [];
        }

        var targetStatus = string.Join(" or ", targetStatuses);
        throw new PluginApplicationException($"No target files found for submission ID {submissionId} that are in {targetStatus} status after {MaxRetries} retries.");
    }

    private async Task<string> ResolveReopenWorkflowIdAsync(string projectId, string workflowId, string? requestedReopenWorkflowId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/projects/{projectId}/workflows/{workflowId}", Method.Get, Credentials);
        var workflow = await Client.ExecuteWithErrorHandling<WorkflowDto>(apiRequest);

        if (!string.IsNullOrEmpty(requestedReopenWorkflowId))
        {
            return requestedReopenWorkflowId;
        }

        if (workflow.LinkedWorkflows.Count == 0)
        {
            throw new PluginApplicationException($"Workflow '{workflow.Name}' (ID {workflow.Id}) has no reopen workflow linked to it, so its targets cannot be reopened. Please configure a reopen workflow in GLE, or contact your GlobalLink administrator.");
        }

        if (workflow.LinkedWorkflows.Count > 1)
        {
            var options = string.Join(", ", workflow.LinkedWorkflows.Select(w => $"{w.Name} (ID {w.Id})"));
            throw new PluginApplicationException($"Workflow '{workflow.Name}' (ID {workflow.Id}) has multiple reopen workflows linked to it. Please specify one via the 'Reopen workflow ID' parameter: {options}.");
        }

        return workflow.LinkedWorkflows[0].Id.ToString();
    }

    private async Task<List<string>> GetProcessedTargetIdsAsync(string submissionId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/targets", Method.Get, Credentials)
            .AddQueryParameter("targetStatus", "PROCESSED")
            .AddQueryParameter("submissionIds", submissionId);

        var targets = await Client.ExecuteWithErrorHandling<List<TargetResponse>>(apiRequest);
        return targets.Select(t => t.TargetId).ToList();
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
