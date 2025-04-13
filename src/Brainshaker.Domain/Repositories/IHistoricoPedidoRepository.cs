namespace Brainshaker.Domain.Repositories;

public interface IHistoricoPedidoRepository
{
    Task<decimal> GetHistoricoByIdAsync(int id);
}