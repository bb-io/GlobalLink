using System;
using Apps.GlobalLink.Handlers;
using Apps.GlobalLink.Models.Requests.Projects;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink.DataHandlerTests;

[TestClass]
public class SourceLanguageDataHandlerTests : BaseDataHandlerTests
{
    protected override IAsyncDataSourceItemHandler DataHandler => new SourceLanguageDataHandler(InvocationContext, new ProjectRequest { ProjectId = "188" });

    protected override string SearchString => "English";
}
