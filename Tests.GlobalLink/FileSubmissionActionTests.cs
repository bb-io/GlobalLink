using Apps.GlobalLink.Actions;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.GlobalLink.Base;

namespace Tests.GlobalLink;

[TestClass]
public class FileSubmissionActionTests : TestBase
{
    [TestMethod]
    public async Task UploadSourceFileAsync_WithValidData_ShouldReturnUploadResponse()
    {
        // Arrange
        var submissionActions = new FileSubmissionAction(InvocationContext, FileManager);
        var submissionId = "17908";
        var fileName = "test_source_file.txt";
        
        var request = new UploadSourceFileRequest
        {
            SubmissionId = submissionId,
            File = new FileReference { Name = fileName }
        };

        // Act
        var result = await submissionActions.UploadSourceFileAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.ProcessId));
        Assert.AreEqual(submissionId, result.SubmissionId);
        Assert.IsNotNull(result.DocumentIds);
        Assert.IsTrue(result.DocumentIds.Count > 0);
        Assert.IsFalse(string.IsNullOrEmpty(result.DocumentIds[0].Name));
        
        // Log results
        Console.WriteLine($"Uploaded file to submission: {JsonConvert.SerializeObject(result, Formatting.Indented)}");
    }

    [TestMethod]
    public async Task UploadReferenceFileAsync_WithValidData_ShouldUploadSuccessfully()
    {
        // Arrange
        var submissionActions = new FileSubmissionAction(InvocationContext, FileManager);
        var submissionId = "17908";
        var fileName = "test_reference_file.txt";
        
        var request = new UploadReferenceFileRequest
        {
            SubmissionId = submissionId,
            File = new FileReference { Name = fileName }
        };

        // Act & Assert
        await submissionActions.UploadReferenceFileAsync(request);
        Console.WriteLine($"Successfully uploaded reference file '{fileName}' to submission with ID: {submissionId}");
    }

    [TestMethod]
    public async Task DownloadTargetFilesAsync_WithValidData_ShouldReturnFiles()
    {
        // Arrange
        var submissionActions = new FileSubmissionAction(InvocationContext, FileManager);
        var submissionId = "7363";
        
        var request = new DownloadTargetFilesRequest
        {
            SubmissionId = submissionId
        };

        // Act
        var result = await submissionActions.DownloadTargetFilesAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.TargetFiles);
        Assert.IsTrue(result.TargetFiles.Any(), "No files were downloaded");
        
        // Log results
        Console.WriteLine($"Downloaded {result.TargetFiles.Count()} files from submission {submissionId}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task UploadTargetFileAsync_WithValidData_ShouldUploadSuccessfully()
    {
        // Arrange
        var submissionActions = new FileSubmissionAction(InvocationContext, FileManager);
        var submissionId = "17896"; 
        var fileName = "3 random sentences-Non-Parsable-es-ES#TR_FQQS#.txt";
        
        var request = new UploadTargetFileRequest
        {
            SubmissionId = submissionId,
            File = new FileReference { Name = fileName }
        };

        // Act & Assert (no exceptions should be thrown)
        await submissionActions.UploadTargetFileAsync(request);
        
        // Log success
        Console.WriteLine($"Successfully uploaded target file '{fileName}' to submission with ID: {submissionId}");
    }

    [TestMethod]
    public async Task DownloadSourceFilesAsync_WithValidData_ShouldReturnFiles()
    {
        // Arrange
        var submissionActions = new FileSubmissionAction(InvocationContext, FileManager);
        var submissionId = "7363";
        var phaseName = "completed";
        
        var request = new DownloadSourceFilesRequest
        {
            SubmissionId = submissionId,
            PhaseName = phaseName
        };

        // Act
        var result = await submissionActions.DownloadSourceFilesAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.SourceFiles);
        Assert.IsTrue(result.SourceFiles.Any(), "No source files were downloaded");
        
        // Log results
        Console.WriteLine($"Downloaded {result.SourceFiles.Count()} source files from submission {submissionId} for phase {phaseName}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task DownloadSourceFilesAsync_WithCompletedSubmission_ShouldFailWithProperErrorMessage()
    {
        // Arrange
        var submissionActions = new FileSubmissionAction(InvocationContext, FileManager);
        var submissionId = "18036";
        var phaseName = "completed";
        
        var request = new DownloadSourceFilesRequest
        {
            SubmissionId = submissionId,
            PhaseName = phaseName
        };

        // Act
        var exception = await Assert.ThrowsExceptionAsync<PluginMisconfigurationException>(async () =>
        {
            await submissionActions.DownloadSourceFilesAsync(request);
        });

        // Assert
        Assert.IsNotNull(exception);
        Assert.AreEqual("The submission is already processed. You cannot download source files from a processed submission. Please create a new submission to upload source files.", exception.Message);
    }
}
