using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegracaoDevOps.Data.Models;

[Table("ts_upgdevops")]
public class UpgradeDevops
{
    [Key] 
    public int PkId { get; set; }
    public int? FkItemTrabalhoAzure { get; set; }
    public string? DsTitulo { get; set; }
    public string? DsTipo { get; set; }
    public string? DsDescricao { get; set; }
    public string? DsEstado { get; set; }
    public string? DsMotivo { get; set; }
    public string? DsTags { get; set; }
    public string? DsSolicitanteNome { get; set; }
    public string? DsSolicitanteEmail { get; set; }
  //  public string? FkSolicitanteIdAzure { get; set; }
    public string? DsProjetoNome { get; set; }
    public string? DsCaminhoArea { get; set; }
    public string? DsCaminhoIteracao { get; set; }
    public string? DsResponsavelNome { get; set; }
    public string? DsResponsavelEmail { get; set; }
    public int? NrPrioridade { get; set; }
    public string? DsUrlUi { get; set; }
    public string? DsUrlApi { get; set; }
    public byte? TgInativo { get; set; }
    public int? FkOwner { get; set; }
    public DateTime? DhInclusao { get; set; }
    public DateTime? DhAlteracao { get; set; }
}

