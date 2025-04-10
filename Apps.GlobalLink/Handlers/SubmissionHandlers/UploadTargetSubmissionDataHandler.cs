using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GlobalLink.Handlers.SubmissionHandlers;

public class UploadTargetSubmissionDataHandler(InvocationContext invocationContext) : SubmissionDataHandler(invocationContext)
{
    public override List<string> AllowedStatuses =>
    [
        "IN_PROCESS",
        "PROCESSING",
        "IN_ERROR",
    ];
}
