using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Handlers.Static;

public class SubmissionStatusStaticDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem("CANCELLED", "Cancelled"),
            new DataSourceItem("CREATING", "Creating"),
            new DataSourceItem("PREFLIGHT", "Preflight"),
            new DataSourceItem("DELIVERED", "Delivered"),
            new DataSourceItem("IN_PROCESS", "In Process"),
            new DataSourceItem("PROCESSED", "Processed"),
            new DataSourceItem("PROCESSING", "Processing"),
            new DataSourceItem("READY", "Ready"),
            new DataSourceItem("WAITING", "Waiting")
        ];
    }
}