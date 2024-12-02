using InoFarm.Models;
using Microsoft.EntityFrameworkCore;


namespace InoFarm.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UsuarioModel> Usuarios { get; set; }
    public DbSet<FrutaModel> Frutas { get; set; }
    public DbSet<CarrinhoModel> Carrinhos { get; set; }
    public DbSet<CarrinhoItemModel> CarrinhoItens { get; set; }
    public DbSet<GerenteModel> Gerentes { get; set; }
    public DbSet<RegistroVendaModels> EntradaSaida { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacionamento de um usuário para muitos carrinhos
        modelBuilder.Entity<UsuarioModel>()
            .HasMany(u => u.Carrinhos)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento de um carrinho para muitos carrinhoItens
        modelBuilder.Entity<CarrinhoModel>()
            .HasMany(c => c.Itens)
            .WithOne(ci => ci.Carrinho)
            .HasForeignKey(ci => ci.IdCarrinho)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento de carrinhoItem com fruta
        modelBuilder.Entity<CarrinhoItemModel>()
            .HasOne(ci => ci.Fruta)
            .WithMany()
            .HasForeignKey(ci => ci.IdFruta)
            .OnDelete(DeleteBehavior.Restrict); // Não é permitido deletar uma fruta enquanto ela estiver no carrinho

        // Relacionamento de carrinhoItem com carrinho
        modelBuilder.Entity<CarrinhoItemModel>()
            .HasOne(ci => ci.Carrinho)
            .WithMany(c => c.Itens)
            .HasForeignKey(ci => ci.IdCarrinho)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento entre Gerente e Fruta (Gerente pode gerenciar várias frutas)
        modelBuilder.Entity<GerenteModel>()
            .HasMany(g => g.FrutasGerenciadas)
            .WithOne(f => f.Gerente)  // Agora definindo o relacionamento em FrutaModel
            .HasForeignKey(f => f.GerenteId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento entre Gerente e Usuario (Gerente pode gerenciar vários usuários)
        modelBuilder.Entity<GerenteModel>()
            .HasMany(g => g.UsuariosGerenciados)
            .WithOne(u => u.Gerente)  // Definindo o relacionamento com UsuarioModel
            .HasForeignKey(u => u.GerenteId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
