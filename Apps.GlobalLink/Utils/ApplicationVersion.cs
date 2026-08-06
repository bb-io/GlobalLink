using System.Reflection;
using Apps.GlobalLink.Constants;

namespace Apps.GlobalLink.Utils;

public static class ApplicationVersion
{
    private const string FallbackVersion = "1.0.0";
    
    public static string Current { get; } = Resolve();

    private static string Resolve()
    {
        var assembly = typeof(ApplicationVersion).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString();

        if (string.IsNullOrWhiteSpace(version))
        {
            return FallbackVersion;
        }

        var buildMetadataIndex = version.IndexOf('+');
        if (buildMetadataIndex > 0)
        {
            version = version[..buildMetadataIndex];
        }

        return version.Length > TechTrackingConstants.MaxFieldLength
            ? version[..TechTrackingConstants.MaxFieldLength]
            : version;
    }
}
