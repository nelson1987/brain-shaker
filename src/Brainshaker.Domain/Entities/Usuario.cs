namespace Brainshaker.Domain.Entities;

public static class Constants
{
    public static string UsuarioTipoCliente = "Cliente";
    public static string UsuarioTipoAdministrador = "Administrador";
}

#region Fase 1 - Admin insere produto

public class Usuario
{
    protected Usuario()
    {
    }

    public Usuario(string nome, string tipo)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Tipo = tipo;
    }

    public Guid Id { get; private set; }

    public string Nome { get; private set; }

    public string Tipo { get; private set; }

    public static Usuario Cliente(string nome)
    {
        return new Usuario(nome, Constants.UsuarioTipoCliente);
    }

    public static Usuario Administrador(string nome)
    {
        return new Usuario(nome, Constants.UsuarioTipoAdministrador);
    }
}

public class Categoria
{
    protected Categoria()
    {
    }

    public Categoria(string nome)
    {
        Nome = nome;
    }

    public string Nome { get; private set; }
}

public class Produto
{
    public Categoria Categoria { get; set; }
    public string Nome { get; set; }
    public decimal CompraPorUnidade { get; set; }
    public decimal MercadoPorUnidade { get; set; }
}

#endregion

#region Fase 2 - Cliente adicionar itens no carrinho

public class Item
{
    protected Item()
    {
    }

    public Item(Produto produto, int quantidade, decimal ultimoValorPorUnidade)
    {
        Produto = produto;
        Quantidade = quantidade;
        UltimoValorPorUnidade = ultimoValorPorUnidade;
    }

    public Produto Produto { get; }
    public int Quantidade { get; }
    public decimal UltimoValorPorUnidade { get; }
    public decimal Estimado => Produto.CompraPorUnidade * Quantidade;
    public decimal PodeVariar => (UltimoValorPorUnidade - Produto.CompraPorUnidade) * Quantidade;
}

public class Compra
{
    public List<Item> Itens { get; set; }
}

#endregion

#region Fase 3 - Admin - Fechou a onde

public class PreCompra
{
    public List<Compra> Carrinhos { get; set; }
    public int QuantidadePorPacote { get; set; }

    public int NumeroDePacotes
    {
        get
        {
            var total = Carrinhos.Sum(y => y.Itens.Sum(x => x.Quantidade));
            var quantidadePacote = Convert.ToDouble(QuantidadePorPacote);
            var totalPacote = Convert.ToDouble(total) / quantidadePacote;
            var arredondamento = Math.Round(totalPacote, MidpointRounding.ToPositiveInfinity);
            return Convert.ToInt32(Math.Round(Convert.ToDouble(total) / Convert.ToDouble(QuantidadePorPacote),
                MidpointRounding.ToPositiveInfinity));
        }
    }

    public int Sobra
    {
        get
        {
            var total = Carrinhos.Sum(y => y.Itens.Sum(x => x.Quantidade));
            return NumeroDePacotes * QuantidadePorPacote - total;
        }
    }
}

public class Abastecimento
{
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorCompra { get; set; }
    public decimal ValorMercado { get; set; }
    public decimal Frete { get; set; }
}

public class Onda
{
    public Abastecimento Abastecimento { get; set; }
    public Item Itens { get; set; }
    public decimal UltimoValorCadastrado => Itens.Produto.CompraPorUnidade;
    public decimal ValorPorUnidadeKg => Abastecimento.ValorCompra / Itens.Quantidade;
    public decimal ValorDeCompra => Itens.Estimado;
    public decimal ValorDeMercadoPorUnidadeKg => Abastecimento.ValorMercado;
    public decimal ValorTotalMercado => ValorDeMercadoPorUnidadeKg * Itens.Quantidade;
    public decimal Economia => ValorTotalMercado - ValorDeCompra;
    public decimal Frete => Abastecimento.Frete;
    public decimal Lucro => Economia * 0.3M;
    public decimal Faturamento => Lucro + ValorDeCompra + Frete; // TODO: o frete deverá ser dividido

    // Ultimo Valor Cadastrado equivale ao Valor de Compra por Unidade do item no carrinho de compra
    // Valor da Unidade Kg é um valor calculado após a realização da compra pelo admin baseando no valor por kg comprado
    // Valor de compra é o valor anterior baseando na quantidade de item comprado
    // Valor de mercado vai ser input pelo Admin após a realziação da compra pelo admin
    // valor de mercado por unikg é o valor anterior baseando na quantidade de itens comprados
    // economia é a subtração entre valor de mercado unidade kg por valor de compra por unidade kg
    // frate que será inputa pelo admin após a realização da compra divido por todos os itens da onda
    // percentual de lucro 30% = ((economia/0.3) * quantidade)
    // Valor faturado - valor da compra efetuada pelo admin + percentual de lucro, porém será adiciona no fechamento da onda
    // o valor do frate baseando-se em dividir o valor inputado pelo admin como frete, entre todos os pedidos da onda
}

#endregion

#region Fase 4 - Criacao Historico

public class HistoricoPedido
{
    public Produto Produto { get; set; }
    public decimal ValorCompraUnidade { get; set; }
    public decimal ValorMercadoUnidade { get; set; }
}

#endregion