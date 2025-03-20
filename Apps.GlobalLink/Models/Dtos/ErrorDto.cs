namespace Apps.GlobalLink.Models.Dtos;

public class ErrorDto
{
    public string[] Messages { get; set; } = Array.Empty<string>();

    public override string ToString()
    {
        return string.Join(Environment.NewLine, Messages);
    }
}
