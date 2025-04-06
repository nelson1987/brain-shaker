using Brainshaker.Domain.Entities;

namespace Brainshaker.Unit;

public class PreCompraUnitTest1
{
    [Fact]
    public void ComUmItemNoCarrinho()
    {
        /*
         RF 1 -
         Eu, como adminitrador e como aplicação
         Quero consultar histórico do pedido
         RN 1 - A aplicação consultará apenas os 3 últimos históricos de 1 item, para compor valores do pedido
         RN 2 - O administrador consultará todo o histórico de 1 item em todos os pedidos
         */
        List<HistoricoPedido> historicoPedido = new List<HistoricoPedido>()
        {
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
        };
        /*
         RF 1 -
         Eu, apenas como cliente
         Quero criar um carrinho de compras
         RN 1 - O Carrinho de Compras tem que ter pelo menos 1 item, podendo ter vários no itens no total
         */
        Compra carrinho = new Compra();
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
        var ultimoValorPorUnidade = historicoPedido.Slice(0, 3).Sum(x => x.ValorCompraUnidade) / 3;
        Item itemBanana = new Item(Builders.Produto(), 20, ultimoValorPorUnidade);
        Assert.Equal(itemBanana.Estimado, 80.00M);
        Assert.Equal(itemBanana.UltimoValorPorUnidade, 5.00M);
        Assert.Equal(itemBanana.PodeVariar, 20.00M);
        carrinho.Itens = new List<Item>() { itemBanana };
        /*
         RF 1
         Eu, apenas como administrador
         Quero receber o relatório de pré-compra
         RN 1 - O administrador deverá inserir a quantidade por pacote de cada produto da onda fechada
         RN 2 - O sistema calculará o número de pacotes
         RN 3 - O sistema calculará a quantidade de produtos que sobrarão
         */
        PreCompra preCompra = new PreCompra(new List<Compra>() { carrinho }, 12);
        Assert.Equal(preCompra.NumeroDePacotes, 2);
        Assert.Equal(preCompra.Sobra, 4);
    }

    [Fact]
    public void ComUmCarrinhoComDoisItensNoCarrinho()
    {
        List<HistoricoPedido> historicoPedido = new List<HistoricoPedido>()
        {
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
        };
        Compra carrinho = new Compra();
        var ultimoValorPorUnidade = historicoPedido.Slice(0, 3).Sum(x => x.ValorCompraUnidade) / 3;
        Item itemBanana = new Item(Builders.Produto(), 20, ultimoValorPorUnidade);
        carrinho.Itens = new List<Item>() { itemBanana, itemBanana };
        /*
         RF 1
         Eu, apenas como administrador
         Quero receber o relatório de pré-compra
         RN 1 - O administrador deverá inserir a quantidade por pacote de cada produto da onda fechada
         RN 2 - O sistema calculará o número de pacotes
         RN 3 - O sistema calculará a quantidade de produtos que sobrarão
         */
        PreCompra preCompra = new PreCompra(new List<Compra>() { carrinho }, 12);
        Assert.Equal(preCompra.NumeroDePacotes, 4);
        Assert.Equal(preCompra.Sobra, 8);
    }

    [Fact]
    public void ComUmCarrinhoComTresItensNoCarrinho()
    {
        List<HistoricoPedido> historicoPedido = new List<HistoricoPedido>()
        {
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
        };
        Compra carrinho = new Compra();
        var ultimoValorPorUnidade = historicoPedido.Slice(0, 3).Sum(x => x.ValorCompraUnidade) / 3;
        Item itemBanana = new Item(Builders.Produto(), 20, ultimoValorPorUnidade);
        carrinho.Itens = new List<Item>() { itemBanana, itemBanana, itemBanana };
        /*
         RF 1
         Eu, apenas como administrador
         Quero receber o relatório de pré-compra
         RN 1 - O administrador deverá inserir a quantidade por pacote de cada produto da onda fechada
         RN 2 - O sistema calculará o número de pacotes
         RN 3 - O sistema calculará a quantidade de produtos que sobrarão
         */
        PreCompra preCompra = new PreCompra(new List<Compra>() { carrinho }, 12);
        Assert.Equal(preCompra.NumeroDePacotes, 5);
        Assert.Equal(preCompra.Sobra, 0);
    }

    [Fact]
    public void ComDoisCarrinhosComUmItemNoCarrinho()
    {
        List<HistoricoPedido> historicoPedido = new List<HistoricoPedido>()
        {
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
            Builders.HistoricoPedido(),
        };
        var ultimoValorPorUnidade = historicoPedido.Slice(0, 3).Sum(x => x.ValorCompraUnidade);
        Compra carrinho = new Compra();
        Item itemBanana = new Item(Builders.Produto(), 20, ultimoValorPorUnidade);
        carrinho.Itens = new List<Item>() { itemBanana };
        Compra carrinho2 = new Compra();
        carrinho2.Itens = new List<Item>() { itemBanana };

        var carrinhos = new List<Compra>() { carrinho, carrinho2 };

        PreCompra preCompra = new PreCompra(carrinhos, 12);
        Assert.Equal(preCompra.NumeroDePacotes, 4);
        Assert.Equal(preCompra.Sobra, 8);
    }
}