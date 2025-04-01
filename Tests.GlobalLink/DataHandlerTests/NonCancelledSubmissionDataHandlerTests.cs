using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class NonCancelledSubmissionDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new NonCancelledSubmissionDataHandler(InvocationContext);

    protected override string SearchString => "test";
}
