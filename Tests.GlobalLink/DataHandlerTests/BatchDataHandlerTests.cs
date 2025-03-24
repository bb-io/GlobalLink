using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class BatchDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new BatchDataHandler(InvocationContext, new SubmissionRequest
    {
        SubmissionId = "17896"
    });

    protected override string SearchString => "Batch";
}
