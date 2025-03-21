using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class ClaimableSubmissionDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new ClaimableSubmissionDataHandler(InvocationContext);

    protected override string SearchString => "first";

    protected override bool CanBeEmpty => true;
}
