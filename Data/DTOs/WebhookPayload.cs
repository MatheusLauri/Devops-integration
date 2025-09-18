using System.Text.Json.Serialization;
using IntegracaoDevOps.Converters; 

namespace IntegracaoDevOps.Data.DTOs;

public class WebhookPayload
{
    [JsonPropertyName("eventType")]
    public string EventType { get; set; }

    [JsonPropertyName("resource")]
    public Resource Resource { get; set; }
}

public class Resource
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fields")]
    public Fields Fields { get; set; }

    [JsonPropertyName("_links")]
    public Links Links { get; set; }
}

public class Fields
{
    [JsonPropertyName("System.WorkItemType")]
    public string WorkItemType { get; set; }

    [JsonPropertyName("System.Title")]
    public string Title { get; set; }

    [JsonPropertyName("System.Description")]
    public string Description { get; set; }

    [JsonPropertyName("System.State")]
    public string State { get; set; }

    [JsonPropertyName("System.Reason")]
    public string Reason { get; set; }

    [JsonPropertyName("System.Tags")]
    public string Tags { get; set; }

    [JsonPropertyName("System.CreatedBy")]
    [JsonConverter(typeof(UserInfoConverter))]
    public UserInfo CreatedBy { get; set; }

    [JsonPropertyName("System.AssignedTo")]
    [JsonConverter(typeof(UserInfoConverter))]
    public UserInfo AssignedTo { get; set; }

    [JsonPropertyName("System.TeamProject")]
    public string TeamProject { get; set; }

    [JsonPropertyName("System.AreaPath")]
    public string AreaPath { get; set; }

    [JsonPropertyName("System.IterationPath")]
    public string IterationPath { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.Priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("System.CreatedDate")]
    public DateTime CreatedDate { get; set; }

}

public class UserInfo
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("uniqueName")]
    public string UniqueName { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class Links
{
    [JsonPropertyName("html")]
    public Link Html { get; set; }

    [JsonPropertyName("self")]
    public Link Self { get; set; }
}

public class Link
{
    [JsonPropertyName("href")]
    public string Href { get; set; }
}