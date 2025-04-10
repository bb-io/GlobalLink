using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GlobalLink.Handlers;

public class NonCancelledSubmissionDataHandler(InvocationContext invocationContext) : SubmissionDataHandler(invocationContext)
{
    public override List<string> AllowedStatuses => new()
    {
        "CREATING",
        "WAITING",
        "PREFLIGHT",
        "READY",
        "IN_PROCESS",
        "PROCESSING",
        "IN_ERROR",
        "ARCHIVED"
    };
}
