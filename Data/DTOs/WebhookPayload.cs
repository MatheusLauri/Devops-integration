using System.Text.Json.Serialization;

namespace IntegracaoDevOps.Data.DTOs;

// Classe principal
public class WebhookPayload
{
    [JsonPropertyName("eventType")]
    public string? EventType { get; set; }

    [JsonPropertyName("resource")]
    public Resource? Resource { get; set; }
}

// Representa o objeto "resource"
public class Resource
{
    // O ID para eventos de CRIAÇÃO
    [JsonPropertyName("id")]
    public int Id { get; set; }

    // O objeto 'revision' para eventos de ATUALIZAÇÃO
    [JsonPropertyName("revision")]
    public RevisionInfo? Revision { get; set; }

    // O objeto 'fields' que contém o estado
    [JsonPropertyName("fields")]
    public Fields? Fields { get; set; }
}

// Classe para capturar o ID dentro de 'revision'
public class RevisionInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}

// Representa o objeto "fields"
public class Fields
{
    [JsonPropertyName("System.State")]
    public FieldChange? State { get; set; }
}

// Classe para capturar a mudança de um campo
public class FieldChange
{
    [JsonPropertyName("newValue")]
    public string? NewValue { get; set; }
}