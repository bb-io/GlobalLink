using Apps.GlobalLink.Actions;
using Apps.GlobalLink.Models.Requests.Submissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.GlobalLink.Base;

namespace Tests.GlobalLink;

[TestClass]
public class SubmissionActionsTests : TestBase
{
    [TestMethod]
    public async Task CreateSubmissionAsync_WithValidData_ShouldReturnSubmissionResponse()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        var projectId = "188"; 

        var request = new CreateSubmissionRequest
        {
            Name = $"Test Submission {DateTime.Now:yyyy:MM:dd HH:mm:ss}",
            DueDate = DateTime.Now.AddDays(7),
            ProjectId = projectId,
            SourceLanguage = "en-US",
            TargetLanguages = new List<string> { "es-ES", "fr-FR" },
            WorkflowId = "201"
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
        var submissionActions = new SubmissionActions(InvocationContext);
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
    public async Task StartSubmissionAsync_WithValidId_ShouldStartSubmission()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
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
    public async Task ClaimSubmissionAsync_WithValidData_ShouldClaimSuccessfully()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        var submissionId = "17896";
        
        var request = new ClaimSubmissionRequest
        {
            SubmissionId = submissionId,
            PhaseName = "Translation"
        };

        // Act & Assert
        await submissionActions.ClaimSubmissionAsync(request);
        Console.WriteLine($"Successfully claimed submission with ID: {submissionId}, for phase: Translation, with languages: es-ES, fr-FR");
    }

    [TestMethod]
    public async Task SearchSubmissionsAsync_WithValidCriteria_ShouldReturnResults()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        
        var request = new SearchSubmissionsRequest
        {
            DueDateFrom = DateTime.UtcNow.AddDays(-30),
            DueDateTo = DateTime.UtcNow.AddDays(30),
            Status = "CANCELLED" 
        };

        // Act
        var result = await submissionActions.SearchSubmissionsAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Submissions);
        
        // Log results
        Console.WriteLine($"Found {result.TotalCount} submissions matching the criteria");
        Console.WriteLine($"Search results: {JsonConvert.SerializeObject(result.Submissions.Take(5), Formatting.Indented)}");
        
        // Verify submissions meet criteria
        if (result.TotalCount > 0)
        {
            foreach (var submission in result.Submissions)
            {
                if (!string.IsNullOrEmpty(request.Status))
                {
                    Assert.AreEqual(request.Status, submission.Status);
                }
                
                if (request.DueDateFrom.HasValue)
                {
                    Assert.IsTrue(submission.DueDate >= request.DueDateFrom.Value);
                }
                
                if (request.DueDateTo.HasValue)
                {
                    Assert.IsTrue(submission.DueDate <= request.DueDateTo.Value);
                }
            }
        }
    }
    
    [TestMethod]
    public async Task SearchSubmissionsAsync_WithDateStartedCriteria_ShouldReturnMatchingResults()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        
        var request = new SearchSubmissionsRequest
        {
            DateStartedFrom = DateTime.UtcNow.AddDays(-30),
            DateStartedTo = DateTime.UtcNow
        };

        // Act
        var result = await submissionActions.SearchSubmissionsAsync(request);

        // Assert
        Assert.IsNotNull(result);
        
        // Log results
        Console.WriteLine($"Found {result.TotalCount} submissions started within the last 30 days");
        
        if (result.TotalCount > 0)
        {
            Console.WriteLine($"Sample submission: {JsonConvert.SerializeObject(result.Submissions.First(), Formatting.Indented)}");
            
            // Verify date started is within range
            foreach (var submission in result.Submissions)
            {
                if (request.DateStartedFrom.HasValue && submission.Started)
                {
                    Assert.IsTrue(submission.DateStarted >= request.DateStartedFrom.Value);
                }
                
                if (request.DateStartedTo.HasValue && submission.Started)
                {
                    Assert.IsTrue(submission.DateStarted <= request.DateStartedTo.Value);
                }
            }
        }
    }

    
    [TestMethod]
    public async Task SearchSubmissionsAsync_WithoutOwners_ShouldReturnMatchingResults()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        
        var request = new SearchSubmissionsRequest
        {
            ExcludeSubmissionsWithOwnersAssigned = true
        };

        // Act
        var result = await submissionActions.SearchSubmissionsAsync(request);

        // Assert
        Assert.IsNotNull(result);
        
        // Log results
        Console.WriteLine($"Found {result.TotalCount} submissions");
        
        if (result.TotalCount > 0)
        {
            Console.WriteLine($"Sample submission: {JsonConvert.SerializeObject(result.Submissions.Select(x => new{x.SubmissionId, x.Owners}), Formatting.Indented)}");
            result.Submissions.ForEach(submission =>
            {
                Assert.IsTrue(submission.Owners == null || submission.Owners.Count == 0, "Submission has owners assigned when it should not.");
            });
        }
    }
    
    [TestMethod]
    public async Task SearchSubmissionsAsync_WithOwnerCriteria_ShouldReturnMatchingResults()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        var expectedUserId = "576";
        
        var initialRequest = new SearchSubmissionsRequest
        {
            OwnerId = expectedUserId
        };

        // Act
        var result = await submissionActions.SearchSubmissionsAsync(initialRequest);
        
        if (result.TotalCount == 0)
        {
            Assert.Inconclusive("No submissions found to test owner search criteria");
            return;
        }

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Submissions);
        Assert.IsTrue(result.Submissions.All(s => s.Owners?.Any(o => string.Equals(o.UserId, expectedUserId, StringComparison.OrdinalIgnoreCase)) == true),
            $"Not all submissions are owned by user with ID: {expectedUserId}");

        // Log results
        Console.WriteLine($"Found {result.TotalCount} submissions owned by {expectedUserId}");
        if (result.TotalCount > 0)
        {
            Console.WriteLine($"Sample submission: {JsonConvert.SerializeObject(result.Submissions.First(), Formatting.Indented)}");
        }
    }

    [TestMethod]
    public async Task CancelSubmissionAsync_WithValidId_ShouldNotThrowException()
    {
        // Arrange
        var submissionActions = new SubmissionActions(InvocationContext);
        var submissionId = "18035";
        
        var request = new CancelSubmissionRequest
        {
            SubmissionId = submissionId
        };

        // Act & Assert - verify no exception is thrown
        await submissionActions.CancelSubmissionAsync(request);
        Console.WriteLine($"Successfully cancelled submission with ID: {submissionId}");
    }
}
