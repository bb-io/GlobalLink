using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Models.Requests.Projects;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class LanguageDirectionsDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new TargetLanguageDataHandler(InvocationContext, new ProjectRequest { ProjectId = "188" });

    protected override string SearchString => "French";
}
