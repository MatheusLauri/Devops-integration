using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegracaoDevOps.Data.DTOs;

public class WebhookPayload
{
    [JsonPropertyName("eventType")]
    public string? EventType { get; set; }
    [JsonPropertyName("resource")]
    public Resource? Resource { get; set; }
}

public class Resource
{
    [JsonPropertyName("revision")]
    public RevisionInfo? Revision { get; set; }
    [JsonPropertyName("fields")]
    public Fields? Fields { get; set; }
}

public class RevisionInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("fields")]
    public Fields? Fields { get; set; }
}

public class Fields
{
    [JsonPropertyName("System.State")]
    public JsonElement? State { get; set; }

    [JsonPropertyName("System.Title")]
    public string? Title { get; set; }
    [JsonPropertyName("System.Description")]
    public string? Description { get; set; }
}