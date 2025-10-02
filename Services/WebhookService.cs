using System.Text.Json;
using System.Text.RegularExpressions;
using IntegracaoDevOps.Data;
using IntegracaoDevOps.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IntegracaoDevOps.Services;

public class WebhookService : IWebhookService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(DatabaseContext context, ILogger<WebhookService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProcessWebhookUpdatedAsync(JsonElement payload)
    {
        try
        {
            var resource = payload.GetProperty("resource");
            var revision = resource.GetProperty("revision");
            var fields = revision.GetProperty("fields");

            var eventType = payload.GetProperty("eventType").GetString();
            var workItemType = fields.GetProperty("System.WorkItemType").GetString();

            if (eventType != "workitem.updated" || workItemType != "User Story")
            {
                _logger.LogInformation("Webhook de atualização recebido, mas ignorado. Evento: {EventType}, Tipo: {WorkItemType}", eventType, workItemType);
                return;
            }

            var azureWorkItemId = revision.GetProperty("id").GetInt32();

            var existingWorkItem = await _context.UpgradeDevopsItems
                .FirstOrDefaultAsync(item => item.FkItemTrabalhoAzure == azureWorkItemId);

            if (existingWorkItem == null)
            {
                _logger.LogWarning("Recebido update para User Story com ID {AzureId}, mas ela não existe no banco. Update ignorado.", azureWorkItemId);
                return;
            }

            existingWorkItem.DsCaminhoIteracaoNew = fields.TryGetProperty("System.IterationPath", out var iter) ? iter.GetString() : null;

            if (existingWorkItem.DsCaminhoIteracao == existingWorkItem.DsCaminhoIteracaoNew)
            {
                _logger.LogWarning("Task não foi prolongada.", azureWorkItemId);
                return;
            }

            if (existingWorkItem.TgProrrogar == 1)
            {
                existingWorkItem.TgProrrogar = 2; // Marca como apartir disso foi prorrogada
                await _context.SaveChangesAsync();
                _logger.LogWarning("Task não foi prolongada.", azureWorkItemId);
                return;
            }

            existingWorkItem.DhProrrogacao = DateTime.UtcNow;
            existingWorkItem.DhAlteracao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("User Story {AzureId} atualizada com sucesso. Nova data de prorrogação: {Data}", azureWorkItemId, existingWorkItem.DhProrrogacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao processar o payload de atualização. Payload recebido: {Payload}", payload.ToString());
            throw;
        }
    }
    public async Task ProcessWebhookPayloadAsync(JsonElement payload)
    {
        try
        {
            var resource = payload.GetProperty("resource");
            var fields = resource.GetProperty("fields");

            var eventType = payload.GetProperty("eventType").GetString();
            var workItemType = fields.GetProperty("System.WorkItemType").GetString();

            if (eventType != "workitem.created" || workItemType != "User Story")
            {
                _logger.LogInformation("Webhook recebido, mas ignorado. Evento: {EventType}, Tipo: {WorkItemType}", eventType, workItemType);
                return;
            }

            var azureWorkItemId = resource.GetProperty("id").GetInt32();

            var alreadyExists = await _context.UpgradeDevopsItems
                                              .AnyAsync(item => item.FkItemTrabalhoAzure == azureWorkItemId);

            if (alreadyExists)
            {
                _logger.LogWarning("User Story com ID {AzureId} já existe no banco. Inserção ignorada.", azureWorkItemId);
                return;
            }

            var solicitante = GetUserDetails(fields, "Custom.Requester") ?? GetUserDetails(fields, "System.CreatedBy");
            var responsavel = GetUserDetails(fields, "System.AssignedTo");

            var newWorkItem = new UpgradeDevops
            {
                FkItemTrabalhoAzure = azureWorkItemId,
                DsTitulo = fields.TryGetProperty("System.Title", out var title) ? title.GetString() : null,
                DsTipo = workItemType,
                DsDescricao = fields.TryGetProperty("System.Description", out var desc) ? desc.GetString() : null,
                DsEstado = fields.TryGetProperty("System.State", out var state) ? state.GetString() : null,
                DsMotivo = fields.TryGetProperty("System.Reason", out var reason) ? reason.GetString() : null,
                DsTags = fields.TryGetProperty("System.Tags", out var tags) ? tags.GetString() : null,
                DsSolicitanteNome = solicitante?.Name,
                DsSolicitanteEmail = solicitante?.Email,
                // FkSolicitanteIdAzure = solicitante?.Id,
                DsResponsavelNome = responsavel?.Name,
                DsResponsavelEmail = responsavel?.Email,
                DsProjetoNome = fields.TryGetProperty("System.TeamProject", out var proj) ? proj.GetString() : null,
                DsCaminhoArea = fields.TryGetProperty("System.AreaPath", out var area) ? area.GetString() : null,
                DsCaminhoIteracao = fields.TryGetProperty("System.IterationPath", out var iter) ? iter.GetString() : null,
                NrPrioridade = fields.TryGetProperty("Microsoft.VSTS.Common.Priority", out var prio) && prio.ValueKind == JsonValueKind.Number ? prio.GetInt32() : null,
                DsUrlUi = resource.TryGetProperty("_links", out var links) && links.TryGetProperty("html", out var html) ? html.GetProperty("href").GetString() : null,
                DsUrlApi = resource.TryGetProperty("_links", out links) && links.TryGetProperty("self", out var self) ? self.GetProperty("href").GetString() : null,
                DhInclusao = fields.TryGetProperty("System.CreatedDate", out var date) ? date.GetDateTime() : null,
                TgInativo = 0,
                TgProrrogar = 1
            };

            if (!string.IsNullOrEmpty(newWorkItem.DsCaminhoIteracao))
            {
                const string prefixo = "Equipe F6\\";
                int indicePrefixo = newWorkItem.DsCaminhoIteracao.IndexOf(prefixo, StringComparison.OrdinalIgnoreCase);

                if (indicePrefixo != -1)
                {
                    // Extrai a parte da string que vem depois do prefixo
                    string parteRelevante = newWorkItem.DsCaminhoIteracao.Substring(indicePrefixo + prefixo.Length);

                    // Verifica se essa parte relevante contém algum dígito
                    if (parteRelevante.Any(char.IsDigit))
                    {
                        _logger.LogInformation("Parte relevante da iteração '{ParteRelevante}' contém números. TgProrrogar será 2.", parteRelevante);
                        newWorkItem.TgProrrogar = 2;
                    }
                }
            }
            // --- FIM DA LÓGICA ATUALIZADA ---

            var jsonItemToSave = JsonSerializer.Serialize(newWorkItem, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation("Objeto a ser salvo no banco: {JsonItem}", jsonItemToSave);

            _context.UpgradeDevopsItems.Add(newWorkItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User Story {AzureId} inserida com sucesso.", azureWorkItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao processar o payload do webhook. Payload recebido: {Payload}", payload.ToString());
            throw;
        }
    }

    private (string? Name, string? Email, string? Id)? GetUserDetails(JsonElement fields, string propertyName)
    {
        if (!fields.TryGetProperty(propertyName, out var userProperty))
        {
            return null;
        }

        if (userProperty.ValueKind == JsonValueKind.String)
        {
            var userString = userProperty.GetString();
            if (string.IsNullOrWhiteSpace(userString))
            {
                return null;
            }

            var match = Regex.Match(userString, @"^(.*?) <(.*?)>$");
            if (match.Success)
            {
                return (match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim(), null);
            }
            return (userString, null, null);
        }

        if (userProperty.ValueKind == JsonValueKind.Object)
        {
            var name = userProperty.TryGetProperty("displayName", out var dn) ? dn.GetString() : null;
            var email = userProperty.TryGetProperty("uniqueName", out var un) ? un.GetString() : null;
            var id = userProperty.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            return (name, email, id);
        }
        return null;
    }
}

