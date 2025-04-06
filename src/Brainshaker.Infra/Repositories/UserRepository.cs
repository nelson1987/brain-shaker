using Brainshaker.Domain.Entities;
using Brainshaker.Domain.Repositories;
using Brainshaker.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace Brainshaker.Infra.Repositories;

public class UserRepository(DatabaseContext context) : AbstractRepository<Usuario>(context)
{
}

public class ProdutoRepository(DatabaseContext context) : AbstractRepository<Produto>(context)
{
}

public class CompraRepository(DatabaseContext context) : AbstractRepository<Compra>(context)
{
}

public class HistoricoPedidoRepository(DatabaseContext context)
    : AbstractRepository<HistoricoPedido>(context), IHistoricoPedidoRepository
{
    public async Task<decimal> GetHistoricoByIdAsync(Guid id)
    {
        List<HistoricoPedido> historicoPedido =
            await context.HistoricosPedidos.Where(x => x.Produto.Id == id).ToListAsync();

        if (historicoPedido.Count < 3)
            return historicoPedido[..historicoPedido.Count].Average(x => x.ValorCompraUnidade);

        return historicoPedido[..3].Average(x => x.ValorCompraUnidade);
    }
}