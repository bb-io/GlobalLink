using System;

namespace Apps.GlobalLink.Models.Requests.Submissions;

public class PhaseResponse
{
    [Newtonsoft.Json.JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("transitions")]
    public List<PhaseTransition> Transitions { get; set; } = new();

    [Newtonsoft.Json.JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("phaseDueDate")]
    public long PhaseDueDate { get; set; }

    [Newtonsoft.Json.JsonProperty("documentId")]
    public string DocumentId { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("targetId")]
    public string TargetId { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("useProtectedPayload")]
    public bool UseProtectedPayload { get; set; }

    [Newtonsoft.Json.JsonProperty("md5Checksum")]
    public string Md5Checksum { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("workflowName")]
    public string WorkflowName { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("phaseOwner")]
    public PhaseOwner PhaseOwner { get; set; } = new();

    [Newtonsoft.Json.JsonProperty("currentPhase")]
    public string CurrentPhase { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("targetFileName")]
    public string TargetFileName { get; set; } = string.Empty;
}

public class PhaseTransition
{
    [Newtonsoft.Json.JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("default")]
    public bool Default { get; set; }

    [Newtonsoft.Json.JsonProperty("targetPhase")]
    public string TargetPhase { get; set; } = string.Empty;
}

public class PhaseOwner
{
    [Newtonsoft.Json.JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}
