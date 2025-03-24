using System.Text;
using Newtonsoft.Json;

namespace Apps.GlobalLink.Models.Dtos;

public class TranslatableUploadProcessDto
{
    [JsonProperty("messages")]
    public List<string> Messages { get; set; } = new();

    [JsonProperty("processingFinished")]
    public bool ProcessingFinished { get; set; }

    public bool HasProcessingFailed => ProcessingFinished && Messages.Count > 0 && !Messages[0].Contains("Upload successfully!", StringComparison.OrdinalIgnoreCase);

    public bool HasUploadSucceeded => ProcessingFinished && Messages.Count > 0 && Messages[0].Contains("Upload successfully!", StringComparison.OrdinalIgnoreCase);

    public string GetErrorMessage()
    {
        var stringBuilder = new StringBuilder();
        foreach (var message in Messages)
        {
            stringBuilder.AppendLine(message);
        }
        
        return stringBuilder.ToString();
    }
}
