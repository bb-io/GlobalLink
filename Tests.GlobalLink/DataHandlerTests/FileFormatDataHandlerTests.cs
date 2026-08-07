using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Models.Requests.Submissions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class FileFormatDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new FileFormatDataHandler(InvocationContext, new SubmissionRequest
    {
        SubmissionId = "17908"
    });

    protected override string SearchString => "JSON";
}
