using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.GlobalLink.Models.Dtos;

public class TechTrackingDto
{
    [JsonProperty("adaptorName"), Display("Adaptor name")]
    public string? AdaptorName { get; set; }

    [JsonProperty("adaptorVersion"), Display("Adaptor version")]
    public string? AdaptorVersion { get; set; }

    [JsonProperty("clientVersion"), Display("Client version")]
    public string? ClientVersion { get; set; }

    [JsonProperty("technologyProduct"), Display("Technology product")]
    public string? TechnologyProduct { get; set; }
}
