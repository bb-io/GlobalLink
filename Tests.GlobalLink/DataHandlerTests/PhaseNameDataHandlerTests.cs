using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class PhaseNameDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new PhaseNameDataHandler(InvocationContext, new() { SubmissionId = "17908"});

    protected override string SearchString => "Translation";

    protected override bool CanBeEmpty => true;
}
