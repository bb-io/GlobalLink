using Apps.GlobalLink.Actions;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;
using Tests.GlobalLink.Base;

namespace Tests.GlobalLink;

[TestClass]
public class SubmissionActionsTests : TestBase
{
    [TestMethod]
    public async Task CreateSubmissionAsync_WithValidData_ShouldReturnSubmissionResponse()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext, FileManager);
        var projectId = "188"; 

        var request = new CreateSubmissionRequest
        {
            Name = $"Test Submission {DateTime.Now:yyyy:MM:dd HH:mm:ss}",
            DueDate = DateTime.Now.AddDays(7),
            ProjectId = projectId,
            SourceLanguage = "en-US",
            TargetLanguages = new List<string> { "es-ES", "fr-FR" },
            WorkflowId = "201",
            WebhookUrl = "https://webhook.site/3ce7bc72-9dfe-4269-b4c3-a5f21a9780cd",
            WebhookScope = "submission.completed"
        };

        // Act
        var result = await submissionActions.CreateSubmissionAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.SubmissionId));
        Assert.IsFalse(string.IsNullOrEmpty(result.Name));
        Assert.AreEqual(request.Name.Replace(":", "_"), result.Name);
        
        // Log results
        Console.WriteLine($"Created submission: {JsonConvert.SerializeObject(result, Formatting.Indented)}");
    }

    [TestMethod]
    public async Task GetSubmissionAsync_WithValidId_ShouldReturnSubmissionResponse()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext, FileManager);
        var submissionId = "17908"; 

        var request = new SubmissionRequest
        {
            SubmissionId = submissionId
        };

        // Act
        var result = await submissionActions.GetSubmissionAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(submissionId, result.SubmissionId);
        Assert.IsFalse(string.IsNullOrEmpty(result.Name));
        Assert.IsFalse(string.IsNullOrEmpty(result.ProjectId));
        
        // Log results for debugging
        Console.WriteLine($"Retrieved submission: {JsonConvert.SerializeObject(result, Formatting.Indented)}");
    }

    [TestMethod]
    public async Task UploadSourceFileAsync_WithValidData_ShouldReturnUploadResponse()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext, FileManager);
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
    public async Task StartSubmissionAsync_WithValidId_ShouldStartSubmission()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext, FileManager);
        var submissionId = "17908"; 

        var request = new SubmissionRequest
        {
            SubmissionId = submissionId
        };

        // Act & Assert
        await submissionActions.StartSubmissionAsync(request);
        Console.WriteLine($"Successfully started submission with ID: {submissionId}");
    }

    [TestMethod]
    public async Task UploadReferenceFileAsync_WithValidData_ShouldUploadSuccessfully()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext, FileManager);
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
    public async Task ClaimSubmissionAsync_WithValidData_ShouldClaimSuccessfully()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext, FileManager);
        var submissionId = "17896";
        
        var request = new ClaimSubmissionRequest
        {
            SubmissionId = submissionId,
            PhaseName = "Translation",
            TargetLanguages = new List<string> { "es-ES", "fr-FR" }
        };

        // Act & Assert
        await submissionActions.ClaimSubmissionAsync(request);
        Console.WriteLine($"Successfully claimed submission with ID: {submissionId}, for phase: Translation, with languages: es-ES, fr-FR");
    }
}
