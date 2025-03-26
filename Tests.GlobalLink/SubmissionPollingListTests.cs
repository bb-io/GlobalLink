using Apps.GlobalLink.Events;
using Apps.GlobalLink.Events.Models;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Polling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.GlobalLink.Base;

namespace Tests.GlobalLink;

[TestClass]
public class SubmissionPollingListTests : TestBase
{
    private SubmissionPollingList _pollingList;
    private readonly List<string> _existingIds = ["17908", "17959", "17955"];

    [TestInitialize]
    public void Initialize()
    {
        _pollingList = new SubmissionPollingList(InvocationContext);
    }

    [TestMethod]
    public async Task OnSubmissionsCreatedAsync_WithNullMemory_ShouldReturnCorrectIds()
    {
        // Arrange
        var request = new PollingEventRequest<SubmissionMemory>
        {
            Memory = null,
            PollingTime = DateTime.UtcNow
        };
        var optionalRequest = new SubmissionOptionalRequest();

        // Act
        var response = await _pollingList.OnSubmissionsCreatedAsync(request, optionalRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"Found submissions: {response.Result.TotalCount}");
        Console.WriteLine($"Stored IDs in memory: {string.Join(", ", response.Memory.SubmissionIds)}");
        
        // Verify the IDs in memory match expected ones
        foreach (var id in _existingIds)
        {
            Assert.IsTrue(response.Memory.SubmissionIds.Contains(id), $"Memory should contain ID: {id}");
        }
        
        Assert.IsFalse(response.FlyBird, "FlyBird should be false for first run with null memory");
    }

    [TestMethod]
    public async Task OnSubmissionCompletedAsync_WithNullMemory_ShouldReturnCorrectIds()
    {
        // Arrange
        var request = new PollingEventRequest<SubmissionMemory>
        {
            Memory = null,
            PollingTime = DateTime.UtcNow
        };
        var optionalRequest = new SubmissionOptionalRequest();

        // Act
        var response = await _pollingList.OnSubmissionCompletedAsync(request, optionalRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"Found submissions: {response.Result.TotalCount}");
        Console.WriteLine($"Stored IDs in memory: {string.Join(", ", response.Memory.SubmissionIds)}");
        
        Assert.IsFalse(response.FlyBird, "FlyBird should be false for first run with null memory");
    }

    [TestMethod]
    public async Task OnSubmissionsCreatedAsync_WithExistingMemory_ShouldIdentifyNewSubmissions()
    {
        // Arrange
        var partialIds = _existingIds.Take(2).ToList(); 
        var expectedParsialIdsCount = partialIds.Count;
        var memory = new SubmissionMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-1),
            SubmissionIds = partialIds
        };
        
        var request = new PollingEventRequest<SubmissionMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var optionalRequest = new SubmissionOptionalRequest();

        // Act
        var response = await _pollingList.OnSubmissionsCreatedAsync(request, optionalRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"Previously known IDs: {string.Join(", ", partialIds)}");
        Console.WriteLine($"Updated memory IDs: {string.Join(", ", response.Memory.SubmissionIds)}");
        
        // Verify new IDs were found
        var newIds = response.Result.Submissions.Select(x => x.SubmissionId).ToList();
        Console.WriteLine($"New IDs found: {string.Join(", ", newIds)}");
        
        Assert.IsTrue(response.Memory.SubmissionIds.Count > expectedParsialIdsCount, "Should find additional IDs");
        Assert.IsTrue(response.Result.Submissions.Count() == 1, "Should find one ID");
        Assert.IsTrue(response.FlyBird, "FlyBird should be true when new submissions are found");
    }

    [TestMethod]
    public async Task OnSubmissionCompletedAsync_WithExistingMemory_ShouldIdentifyNewSubmissions()
    {
        // Arrange
        var partialIds = _existingIds.Take(2).ToList(); // Only include some IDs to simulate new submissions
        var memory = new SubmissionMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-1),
            SubmissionIds = partialIds
        };
        
        var request = new PollingEventRequest<SubmissionMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var optionalRequest = new SubmissionOptionalRequest();

        // Act
        var response = await _pollingList.OnSubmissionCompletedAsync(request, optionalRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"Previously known IDs: {string.Join(", ", partialIds)}");
        Console.WriteLine($"Updated memory IDs: {string.Join(", ", response.Memory.SubmissionIds)}");
        
        // Check for new IDs
        var newIds = response.Memory.SubmissionIds.Except(partialIds).ToList();
        Console.WriteLine($"New IDs found: {string.Join(", ", newIds)}");
        
        // FlyBird could be true or false depending on if there are any completed submissions
        Console.WriteLine($"FlyBird: {response.FlyBird}");
    }

    [TestMethod]
    public async Task OnSubmissionsCreatedAsync_WithSpecificSubmissionId_ShouldReturnThatSubmission()
    {
        // Arrange
        var specificId = "17908";
        var request = new PollingEventRequest<SubmissionMemory>
        {
            Memory = null,
            PollingTime = DateTime.UtcNow
        };
        var optionalRequest = new SubmissionOptionalRequest
        {
            SubmissionId = specificId
        };

        // Act
        var response = await _pollingList.OnSubmissionsCreatedAsync(request, optionalRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        
        Assert.AreEqual(1, response.Result.TotalCount, "Should return exactly one submission");
        Assert.AreEqual(specificId, response.Result.Submissions[0].SubmissionId, "Should return the requested submission");
    }

    [TestMethod]
    public async Task OnSubmissionCompletedAsync_WithSpecificSubmissionId_ShouldSetFlyBirdToFalse()
    {
        // Arrange
        var specificId = "17908";
        var memory = new SubmissionMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-1),
            SubmissionIds = new List<string> { specificId }  // Already known ID
        };
        
        var request = new PollingEventRequest<SubmissionMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var optionalRequest = new SubmissionOptionalRequest
        {
            SubmissionId = specificId
        };

        // Act
        var response = await _pollingList.OnSubmissionCompletedAsync(request, optionalRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        
        Assert.IsFalse(response.FlyBird, "FlyBird should be false when submission ID is already known");
    }
}
