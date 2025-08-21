using Microsoft.AspNetCore.Mvc;
using IntegracaoDevOps.Data;
using IntegracaoDevOps.Data.Models;
using IntegracaoDevOps.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IntegracaoDevOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly DatabaseContext _context;

    public WebhookController(ILogger<WebhookController> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("userstory-updated")]
    public async Task<IActionResult> ReceiveUserStoryUpdate([FromBody] WebhookPayload payload)
    {
        if (payload?.EventType != "workitem.updated" || !payload.Resource?.Fields?.State.HasValue == true)
        {
            return Ok("Evento ignorado (não é uma atualização com mudança de estado).");
        }

        try
        {
            string? newState = null;
            var stateElement = payload.Resource.Fields.State.Value;

            if (stateElement.ValueKind == JsonValueKind.Object && stateElement.TryGetProperty("newValue", out var newValueElement))
            {
                newState = newValueElement.GetString();
            }

            if (newState?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (payload.Resource.Revision?.Fields == null)
                {
                    return BadRequest("Payload de atualização incompleto.");
                }

                var workItemId = payload.Resource.Revision.Id;
                var workItemIdStr = workItemId.ToString();

                var alreadyExists = await _context.UpgradeDevops.AnyAsync(item => item.DsUs == workItemIdStr);

                if (!alreadyExists)
                {
                    var fields = payload.Resource.Revision.Fields;

                    var newEntry = new UpgradeDevops
                    {
                        DsUs = workItemIdStr,
                        DsEvento = newState,
                        Title = fields.Title,
                        Description = fields.Description
                    };

                    _context.UpgradeDevops.Add(newEntry);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("REGRA CUMPRIDA: Item #{WorkItemId} ('{Title}') movido para 'In Progress' e salvo no banco.", workItemId, newEntry.Title);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar o webhook.");
            return StatusCode(500, "Internal Server Error");
        }

        return Ok();
    }
}