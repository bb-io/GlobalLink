using Apps.GlobalLink.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Handlers.Static;

public class WebhookScopeStaticDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new(ScopeConstants.SubmissionCompleted, "Submission completed"),
            new(ScopeConstants.SubmissionAnalyzed, "Submission analyzed"),
            new(ScopeConstants.SubmissionCancelled, "Submission cancelled")
            // new("target.completed", "Target completed"),
            // new("target.cancelled", "Target cancelled")
        ];
    }
}
