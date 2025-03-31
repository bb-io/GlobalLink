using Apps.GlobalLink.Events;
using Apps.GlobalLink.Events.Models;
using Apps.GlobalLink.Models.Requests.Projects;
using Blackbird.Applications.Sdk.Common.Polling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tests.GlobalLink.Base;

namespace Tests.GlobalLink;

[TestClass]
public class UserPollingListTests : TestBase
{
    private UserPollingList _pollingList;
    private readonly List<string> _existingIds = ["576", "509", "505"];
    private const string ProjectId = "188";

    [TestInitialize]
    public void Initialize()
    {
        _pollingList = new UserPollingList(InvocationContext);
    }

    [TestMethod]
    public async Task OnUsersCreatedAsync_WithNullMemory_ShouldReturnCorrectIds()
    {
        // Arrange
        var request = new PollingEventRequest<UserMemory>
        {
            Memory = null,
            PollingTime = DateTime.UtcNow
        };
        var projectRequest = new ProjectRequest { ProjectId = ProjectId };

        // Act
        var response = await _pollingList.OnUsersCreatedAsync(request, projectRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"Found users: {response.Result.TotalCount}");
        Console.WriteLine($"Stored IDs in memory: {string.Join(", ", response.Memory.UserIds)}");
        
        // Verify the IDs in memory match expected ones
        foreach (var id in _existingIds)
        {
            Assert.IsTrue(response.Memory.UserIds.Contains(id), $"Memory should contain ID: {id}");
        }
        
        Assert.IsFalse(response.FlyBird, "FlyBird should be false for first run with null memory");
        Assert.AreEqual(0, response.Result.Users.Count, "Should not return any users on first run");
    }

    [TestMethod]
    public async Task OnUsersCreatedAsync_WithExistingMemory_ShouldIdentifyNewUsers()
    {
        // Arrange
        var partialIds = _existingIds.Take(2).ToList(); 
        var expectedPartialIdsCount = partialIds.Count;
        var memory = new UserMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-1),
            UserIds = partialIds
        };
        
        var request = new PollingEventRequest<UserMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var projectRequest = new ProjectRequest { ProjectId = ProjectId };

        // Act
        var response = await _pollingList.OnUsersCreatedAsync(request, projectRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"Previously known IDs: {string.Join(", ", partialIds)}");
        Console.WriteLine($"Updated memory IDs: {string.Join(", ", response.Memory.UserIds)}");
        
        // Verify new IDs were found
        var newIds = response.Result.Users.Select(x => x.UserId).ToList();
        Console.WriteLine($"New IDs found: {string.Join(", ", newIds)}");
        
        Assert.IsTrue(response.Memory.UserIds.Count > expectedPartialIdsCount, "Should find additional IDs");
        Assert.IsTrue(response.Result.Users.Count == 1, "Should find one ID");
        Assert.IsTrue(response.FlyBird, "FlyBird should be true when new users are found");
    }

    [TestMethod]
    public async Task OnUsersCreatedAsync_WithCompleteMemory_ShouldNotFindNewUsers()
    {
        // Arrange
        var memory = new UserMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-1),
            UserIds = _existingIds
        };
        
        var request = new PollingEventRequest<UserMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var projectRequest = new ProjectRequest { ProjectId = ProjectId };

        // Act
        var response = await _pollingList.OnUsersCreatedAsync(request, projectRequest);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Result);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        
        // Check that no new users were found
        Assert.AreEqual(0, response.Result.Users.Count, "Should not find any new users");
        Assert.IsFalse(response.FlyBird, "FlyBird should be false when no new users are found");
    }
}
