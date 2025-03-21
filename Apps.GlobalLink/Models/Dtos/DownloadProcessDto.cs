namespace Apps.GlobalLink.Models.Dtos;

public class DownloadProcessDto
{
    public string Message { get; set; } = string.Empty;

    public bool ProcessingFinished { get; set; }

    public string DownloadId { get; set; } = string.Empty;
}
