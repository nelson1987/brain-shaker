using Brainshaker.Domain.Entities;

namespace Brainshaker.Unit;

public static class Builders
{
    public static Produto Produto()
    {
        Categoria fruta = new Categoria("Fruta");
        return new Produto(Guid.NewGuid(), fruta, "Banana", 4.00M, 7.00M);
    }

    public static HistoricoPedido HistoricoPedido()
    {
        return new HistoricoPedido(Builders.Produto(), 5.00M, 7.00M);
    }
}