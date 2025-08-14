namespace IntegracaoDevOps.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TS_UPGDEVOPS")]
public class UpgradeDevops
{
    [Key]
    [Column("PK_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("DS_US")]
    public string? DsUs { get; set; }

    [Column("DS_EVENTO")]
    public string? DsEvento { get; set; } 
}