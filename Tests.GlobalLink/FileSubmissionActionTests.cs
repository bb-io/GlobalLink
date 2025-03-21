using Apps.GlobalLink.Actions;
using Apps.GlobalLink.Models.Requests.Submissions;
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
        var submissionId = "17855";
        
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
}
