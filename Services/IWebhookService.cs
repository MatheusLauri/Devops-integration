using System.Text.Json;

namespace IntegracaoDevOps.Services;

public interface IWebhookService
{
    Task ProcessWebhookPayloadAsync(JsonElement payload);
    Task ProcessWebhookUpdatedAsync(JsonElement payload);
    Task ProcessWebhookStateUpdatedAsync(JsonElement payload);
}
