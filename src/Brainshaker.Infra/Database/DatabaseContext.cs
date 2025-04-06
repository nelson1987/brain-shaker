using Brainshaker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Brainshaker.Infra.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    #region Fase 1 - Admin insere produto

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    #endregion

    #region Fase 2 - Cliente adicionar itens no carrinho

    public DbSet<Item> Itens { get; set; }
    public DbSet<Compra> Compras { get; set; }

    #endregion

    #region Fase 3 - Admin - Fechou a onde

    public DbSet<PreCompra> PreCompras { get; set; }
    public DbSet<Abastecimento> Abastecimentos { get; set; }
    public DbSet<Onda> Ondas { get; set; }

    #endregion

    #region Fase 4 - Criacao Historico

    public DbSet<HistoricoPedido> HistoricosPedidos { get; set; }

    #endregion
}