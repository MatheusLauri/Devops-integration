using IntegracaoDevOps.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegracaoDevOps.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<UpgradeDevops> UpgradeDevopsItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UpgradeDevops>(entity =>
        {
            entity.ToTable("ts_upgdevops");

            entity.Property(e => e.PkId).HasColumnName("PK_ID");
            entity.Property(e => e.FkItemTrabalhoAzure).HasColumnName("FK_ITEM_TRABALHO_AZURE");
 
            entity.Property(e => e.DsTitulo).HasColumnName("DS_TITULO").HasColumnType("nvarchar(255)");
            entity.Property(e => e.DsTipo).HasColumnName("DS_TIPO").HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.DsDescricao).HasColumnName("DS_DESCRICAO").HasColumnType("ntext");
            entity.Property(e => e.DsEstado).HasColumnName("DS_ESTADO").HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.DsMotivo).HasColumnName("DS_MOTIVO").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.DsTags).HasColumnName("DS_TAGS").HasMaxLength(500).IsUnicode(false);
            entity.Property(e => e.DsSolicitanteNome).HasColumnName("DS_SOLICITANTE_NOME").HasColumnType("nvarchar(255)");
            entity.Property(e => e.DsSolicitanteEmail).HasColumnName("DS_SOLICITANTE_EMAIL").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.DsProjetoNome).HasColumnName("DS_PROJETO_NOME").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.DsCaminhoArea).HasColumnName("DS_CAMINHO_AREA").HasMaxLength(500).IsUnicode(false);
            entity.Property(e => e.DsCaminhoIteracao).HasColumnName("DS_CAMINHO_ITERACAO").HasMaxLength(500).IsUnicode(false);
            entity.Property(e => e.DsResponsavelNome).HasColumnName("DS_RESPONSAVEL_NOME").HasColumnType("nvarchar(255)");
            entity.Property(e => e.DsResponsavelEmail).HasColumnName("DS_RESPONSAVEL_EMAIL").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.NrPrioridade).HasColumnName("NR_PRIORIDADE");
            entity.Property(e => e.DsUrlUi).HasColumnName("DS_URL_UI").HasMaxLength(1024).IsUnicode(false);
            entity.Property(e => e.DsUrlApi).HasColumnName("DS_URL_API").HasMaxLength(1024).IsUnicode(false);
            entity.Property(e => e.TgInativo).HasColumnName("TG_INATIVO");
            entity.Property(e => e.TgProrrogar).HasColumnName("TG_PRORROGAR");
            entity.Property(e => e.FkOwner).HasColumnName("FK_OWNER");
            entity.Property(e => e.DhInclusao).HasColumnName("DH_INCLUSAO");
            entity.Property(e => e.DhAlteracao).HasColumnName("DH_ALTERACAO");
            entity.Property(e => e.DhProrrogacao).HasColumnName("DH_PRORROGACAO");
            entity.Property(e => e.DsCaminhoIteracaoNew).HasColumnName("DS_CAMINHO_ITERACAO_NEW").HasMaxLength(500).IsUnicode(false);
        });
    }
}

