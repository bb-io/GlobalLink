using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GlobalLink.Handlers.SubmissionHandlers;

public class ProcessedSubmissionDataHandler(InvocationContext invocationContext) : SubmissionDataHandler(invocationContext)
{
    public override List<string> AllowedStatuses =>
    [
        "PROCESSED",
        "DELIVERED",
    ];
}
