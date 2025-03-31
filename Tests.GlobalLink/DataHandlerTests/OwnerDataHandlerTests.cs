using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class OwnerDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => 
        new OwnerDataHandler(InvocationContext, new SearchSubmissionsRequest { ProjectId = "188" });

    protected override string SearchString => "vitalii";
}
