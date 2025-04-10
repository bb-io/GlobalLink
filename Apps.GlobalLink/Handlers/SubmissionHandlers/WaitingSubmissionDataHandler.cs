using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.GlobalLink.Handlers.SubmissionHandlers;

public class WaitingSubmissionDataHandler(InvocationContext invocationContext) : SubmissionDataHandler(invocationContext)
{
    public override List<string> AllowedStatuses =>
    [
        "WAITING"
    ];
}
