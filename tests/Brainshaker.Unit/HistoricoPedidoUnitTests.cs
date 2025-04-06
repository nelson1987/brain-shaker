using Brainshaker.Domain.Entities;

namespace Brainshaker.Unit;

public class HistoricoPedidoUnitTests
{
    [Fact]
    public void BuscarMedia_Historico_Vazio()
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
        };
        throw new NotImplementedException();
        // var media = historicoPedido.Slice(0, 3).Average(x => x.ValorCompraUnidade);
        // Assert.Equal(5.00M, media);
    }

    [Fact]
    public void BuscarMedia_Historico_ComUm()
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
        };
        if (historicoPedido.Count < 3)
        {
            var media = historicoPedido[..historicoPedido.Count].Average(x => x.ValorCompraUnidade);
            Assert.Equal(5.00M, media);
        }
    }

    [Fact]
    public void BuscarMedia_Historico_ComDois()
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
        };
        if (historicoPedido.Count < 3)
        {
            var media = historicoPedido[..historicoPedido.Count].Average(x => x.ValorCompraUnidade);
            Assert.Equal(5.00M, media);
        }
    }

    [Fact]
    public void BuscarMedia_Historico_ComTres()
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
        var media = historicoPedido.Slice(0, 3).Average(x => x.ValorCompraUnidade);
        Assert.Equal(5.00M, media);
    }

    [Fact]
    public void BuscarMedia_Historico_ComMaisDeTres()
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
            Builders.HistoricoPedido()
        };
        var media = historicoPedido.Slice(0, 3).Average(x => x.ValorCompraUnidade);
        Assert.Equal(5.00M, media);
    }
}