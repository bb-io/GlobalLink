using Apps.GlobalLink.Api;
using Apps.GlobalLink.Events.Models;
using Apps.GlobalLink.Models.Requests.Projects;
using Apps.GlobalLink.Models.Responses;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.GlobalLink.Events;

[PollingEventList]
public class UserPollingList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [PollingEvent("On users created", Description = "Polling event that periodically checks for new users added to the organization.")]
    public async Task<PollingEventResponse<UserMemory, SearchUsersResponse>> OnUsersCreatedAsync(
        PollingEventRequest<UserMemory> request,
        [PollingEventParameter] ProjectRequest projectRequest)
    {
        var users = await FetchUsersAsync(projectRequest.ProjectId);
        if (request.Memory == null)
        {
            return CreateInitialResponse(users);
        }

        return CreatePollingResponse(request.Memory, users);
    }

    private async Task<List<UserResponse>> FetchUsersAsync(string projectId)
    {
        var apiRequest = new ApiRequest($"/rest/v0/projects/{projectId}/users/org", Method.Get, Credentials);
        return await Client.PaginateAsync<UserResponse>(apiRequest);
    }

    private PollingEventResponse<UserMemory, SearchUsersResponse> CreateInitialResponse(List<UserResponse> users)
    {
        return new PollingEventResponse<UserMemory, SearchUsersResponse>
        {
            Result = new SearchUsersResponse(new List<UserResponse>()),
            Memory = CreateUserMemory(users),
            FlyBird = false
        };
    }

    private PollingEventResponse<UserMemory, SearchUsersResponse> CreatePollingResponse(UserMemory memory, List<UserResponse> currentUsers)
    {
        var newUsers = IdentifyNewUsers(memory.UserIds, currentUsers);
        
        return new PollingEventResponse<UserMemory, SearchUsersResponse>
        {
            Result = new SearchUsersResponse(newUsers),
            Memory = CreateUserMemory(currentUsers),
            FlyBird = newUsers.Any()
        };
    }

    private List<UserResponse> IdentifyNewUsers(List<string> existingUserIds, List<UserResponse> currentUsers)
    {
        return currentUsers
            .Where(user => !existingUserIds.Contains(user.UserId))
            .ToList();
    }

    private UserMemory CreateUserMemory(List<UserResponse> users)
    {
        return new UserMemory
        {
            LastPollingTime = DateTime.UtcNow,
            UserIds = users.Select(user => user.UserId).ToList()
        };
    }
}
