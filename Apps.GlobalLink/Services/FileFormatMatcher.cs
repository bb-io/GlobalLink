using Apps.GlobalLink.Models.Responses.Projects;

namespace Apps.GlobalLink.Services;

public static class FileFormatMatcher
{
    public static string? Match(List<FileFormatResponse> fileFormats, IEnumerable<string> candidateMimeTypes)
    {
        var parsableFileFormats = fileFormats
            .Where(x => x.Enabled && !x.NonParsable && !string.IsNullOrEmpty(x.Name) && x.MimeTypes.Count > 0)
            .ToList();

        if (parsableFileFormats.Count == 0)
        {
            return null;
        }

        return candidateMimeTypes
            .Select(mimeType => MatchSingle(parsableFileFormats, mimeType))
            .FirstOrDefault(match => match != null);
    }

    private static string? MatchSingle(List<FileFormatResponse> fileFormats, string mimeType)
    {
        var matches = fileFormats
            .Where(x => x.MimeTypes.Any(m => string.Equals(m, mimeType, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (matches.Count <= 1)
        {
            return matches.FirstOrDefault()?.Name;
        }
        
        var defaultMatches = matches.Where(x => x.Default).ToList();
        return defaultMatches.Count == 1 ? defaultMatches[0].Name : null;
    }
}
