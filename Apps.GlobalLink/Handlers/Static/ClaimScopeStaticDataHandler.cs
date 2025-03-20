using System;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GlobalLink.Handlers.Static;

public class ClaimScopeStaticDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem("LANGUAGE", "Language"),
            new DataSourceItem("BATCH", "Batch"),
            new DataSourceItem("FILE", "File")
        ];
    }
}
