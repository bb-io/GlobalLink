using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GlobalLink.Handlers.SubmissionHandlers;

public class UploadSourceSubmissionDataHandler(InvocationContext invocationContext) : SubmissionDataHandler(invocationContext)
{
    public override List<string> AllowedStatuses =>
    [
        "IN_PROCESS",
        "PROCESSING",
    ];
}
