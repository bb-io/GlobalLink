using System;
using Apps.GlobalLink.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Tests.GlobalLink.DataHandlerTests;

public class PhaseNameDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new PhaseNameDataHandler(InvocationContext, new() { SubmissionId = ""});

    protected override string SearchString => throw new NotImplementedException();
}
