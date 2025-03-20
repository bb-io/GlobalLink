using System;
using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Models.Requests.Projects;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class WorkflowDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new WorkflowDataHandler(InvocationContext, new ProjectRequest { ProjectId = "188" });

    protected override string SearchString => "Translation_";
}
