namespace Apps.GlobalLink.Models.Dtos;

public class WorkflowDto
{
    public string Name { get; set; } = string.Empty;

    public long Id { get; set; }

    public List<LinkedWorkflowDto> LinkedWorkflows { get; set; } = new();
}

public class LinkedWorkflowDto
{
    public string Name { get; set; } = string.Empty;

    public long Id { get; set; }
}
