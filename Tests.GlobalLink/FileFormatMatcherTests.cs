using Apps.GlobalLink.Models.Responses.Projects;
using Apps.GlobalLink.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.GlobalLink;

[TestClass]
public class FileFormatMatcherTests
{
    private const string WordMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    private const string HtmlMimeType = "text/html";

    [TestMethod]
    public void Match_WithSingleMatchingFileFormat_ShouldReturnItsName()
    {
        var fileFormats = new List<FileFormatResponse>
        {
            CreateFileFormat("MSWORD_TXLF", WordMimeType),
            CreateFileFormat("HTML_TXLF", HtmlMimeType)
        };

        var result = FileFormatMatcher.Match(fileFormats, [WordMimeType]);

        Assert.AreEqual("MSWORD_TXLF", result);
    }

    [TestMethod]
    public void Match_WithSeveralMatchingFileFormats_ShouldReturnTheDefaultOne()
    {
        var fileFormats = new List<FileFormatResponse>
        {
            CreateFileFormat("MSWORD_TXLF_CUSTOM", WordMimeType),
            CreateFileFormat("MSWORD_TXLF", WordMimeType, isDefault: true)
        };

        var result = FileFormatMatcher.Match(fileFormats, [WordMimeType]);

        Assert.AreEqual("MSWORD_TXLF", result);
    }

    [TestMethod]
    public void Match_WithSeveralMatchingFileFormatsAndNoDefaultOne_ShouldReturnNull()
    {
        var fileFormats = new List<FileFormatResponse>
        {
            CreateFileFormat("MSWORD_TXLF_CUSTOM", WordMimeType),
            CreateFileFormat("MSWORD_TXLF", WordMimeType)
        };

        var result = FileFormatMatcher.Match(fileFormats, [WordMimeType]);

        Assert.IsNull(result, "An ambiguous file type must not be resolved to a parser.");
    }

    [TestMethod]
    public void Match_WithFallbackMimeType_ShouldUseTheFirstMatchingAlternative()
    {
        var fileFormats = new List<FileFormatResponse> { CreateFileFormat("HTML_TXLF", HtmlMimeType) };

        var result = FileFormatMatcher.Match(fileFormats, ["application/x-unknown", HtmlMimeType]);

        Assert.AreEqual("HTML_TXLF", result);
    }

    [TestMethod]
    public void Match_WithoutMatchingFileFormat_ShouldReturnNull()
    {
        var fileFormats = new List<FileFormatResponse> { CreateFileFormat("MSWORD_TXLF", WordMimeType) };

        var result = FileFormatMatcher.Match(fileFormats, ["image/jpeg"]);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Match_WithDisabledOrNonParsableFileFormats_ShouldIgnoreThem()
    {
        var fileFormats = new List<FileFormatResponse>
        {
            CreateFileFormat("MSWORD_TXLF_DISABLED", WordMimeType, enabled: false),
            CreateFileFormat("MSWORD_NON_PARSABLE", WordMimeType, nonParsable: true)
        };

        var result = FileFormatMatcher.Match(fileFormats, [WordMimeType]);

        Assert.IsNull(result);
    }

    private static FileFormatResponse CreateFileFormat(
        string name,
        string mimeType,
        bool isDefault = false,
        bool enabled = true,
        bool nonParsable = false)
    {
        return new FileFormatResponse
        {
            Name = name,
            MimeTypes = [mimeType],
            Default = isDefault,
            Enabled = enabled,
            NonParsable = nonParsable,
            TargetFormat = nonParsable ? "NON_PARSABLE" : "TXLF"
        };
    }
}
