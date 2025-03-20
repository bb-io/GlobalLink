using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Handlers.Static;

public class WebhookScopeStaticDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new("submission.completed", "Submission completed"),
            new("submission.cancelled", "Submission cancelled"),
            new("target.completed", "Target completed"),
            new("target.cancelled", "Target cancelled")
        ];
    }
}
