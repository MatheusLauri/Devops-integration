using System.Text.Json;
using IntegracaoDevOps.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntegracaoDevOps.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookService _webhookService;

    public WebhookController(IWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    [HttpPost("userstory-created")]
    public async Task<IActionResult> HandleUserStoryCreated([FromBody] JsonElement payload)
    {
        if (payload.ValueKind == JsonValueKind.Undefined || payload.ValueKind == JsonValueKind.Null)
        {
            return BadRequest("Payload não pode ser nulo.");
        }

        await _webhookService.ProcessWebhookPayloadAsync(payload);

        return Ok();
    }
}
