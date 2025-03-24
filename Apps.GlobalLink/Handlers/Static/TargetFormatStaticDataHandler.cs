using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Handlers.Static;

public class TargetFormatStaticDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new("TXLF", "TXLF"),
            new("NON_PARSABLE", "Non parsable")
        ];
    }
}
