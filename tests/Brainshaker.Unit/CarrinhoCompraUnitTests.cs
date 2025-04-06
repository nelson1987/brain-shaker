using Brainshaker.Domain.Entities;

namespace Brainshaker.Unit;

public class CarrinhoCompraUnitTests
{
    [Fact]
    public void Test()
    {
        Categoria fruta = new Categoria("Fruta");
        Produto banana = new Produto(Guid.NewGuid(), fruta, "Banana", 4.00M, 7.00M);
        /*
         RF 1 -
         Eu, como adminitrador e como aplicação
         Quero consultar histórico do pedido
         RN 1 - A aplicação consultará apenas os 3 últimos históricos de 1 item, para compor valores do pedido
         RN 2 - O administrador consultará todo o histórico de 1 item em todos os pedidos
         */
        List<HistoricoPedido> historicoPedido = new List<HistoricoPedido>()
        {
            new HistoricoPedido(banana, 5.00M, 7.00M),
            new HistoricoPedido(banana, 5.00M, 7.00M),
            new HistoricoPedido(banana, 5.00M, 7.00M)
        };
        /*
         RF 1 -
         Eu, apenas como cliente
         Quero criar um carrinho de compras
         RN 1 - O Carrinho de Compras tem que ter pelo menos 1 item, podendo ter vários no itens no total
         */
        Compra carrinho = new Compra();
        // carrinho.AdicionarItem(itemBanana, historicoPedido);
        /*
         RF 1 -
         Eu, apenas como cliente
         Quero incluir um item ao carrinho de compras
         RN 1 - O item deve ter uma quantidade
         RN 2 - O sistema calculará o último valor por unidade, que será os 3 últimos históricos desse produto nos pedidos, sendo a soma do valor de compra por unidade dividido por 3
         Caso não existam 3 históricos, o valor deve ser calcula da média da quantidade existente. Caso não haja nenhum histórico o valor a ser assumido é o cadastrado no produto pelo administrador.
         RN 3 - O sistema calculará o valor estimado que será (???)
         RN 4 - O sistema calculará o valor de variacao que será (???)
         */
        var ultimoValorPorUnidade = historicoPedido.Slice(0, 3).Average(x => x.ValorCompraUnidade);
        Item itemBanana = new Item(banana, 20, ultimoValorPorUnidade);
        Assert.Equal(itemBanana.Estimado, 80.00M);
        Assert.Equal(itemBanana.UltimoValorPorUnidade, 5.00M);
        Assert.Equal(itemBanana.PodeVariar, 20.00M);
        carrinho.Itens = new List<Item>() { itemBanana };
    }
}