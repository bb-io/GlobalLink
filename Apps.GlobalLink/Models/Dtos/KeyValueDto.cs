using Newtonsoft.Json;

namespace Apps.GlobalLink.Models.Dtos;

public class KeyValueDto
{
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;
    
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;
}
