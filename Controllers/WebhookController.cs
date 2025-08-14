using Microsoft.AspNetCore.Mvc;
using IntegracaoDevOps.Data;
using IntegracaoDevOps.Data.Models;
using IntegracaoDevOps.Data.DTOs;

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
        if (payload?.Resource == null || payload.EventType == null)
        {
            return BadRequest("Payload inválido.");
        }

        try
        {
            string? actionToSave = null;
            int workItemId = 0;

            if (payload.EventType == "workitem.created")
            {
                actionToSave = "created";
                workItemId = payload.Resource.Id;
            }
            else if (payload.EventType == "workitem.updated")
            {
                actionToSave = payload.Resource.Fields?.State?.NewValue;
                if (payload.Resource.Revision != null)
                {
                    workItemId = payload.Resource.Revision.Id;
                }
            }

            if (workItemId > 0 && !string.IsNullOrEmpty(actionToSave))
            {
                var newEntry = new UpgradeDevops
                {
                    DsUs = workItemId.ToString(), // ID da US
                    DsEvento = actionToSave        // Ação do evento
                };

                _context.UpgradeDevops.Add(newEntry);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Item #{Id}: Ação '{Action}' salva com sucesso.", workItemId, actionToSave);
            }
            else
            {
                _logger.LogWarning("Dados insuficientes para salvar o evento '{EventType}'.", payload.EventType);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar e salvar o webhook.");
            return StatusCode(500, "Internal Server Error");
        }

        return Ok();
    }
}