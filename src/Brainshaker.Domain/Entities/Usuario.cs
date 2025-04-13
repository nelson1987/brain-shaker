namespace Brainshaker.Domain.Entities;

public static class Constants
{
    public static string UsuarioTipoCliente = "Cliente";
    public static string UsuarioTipoAdministrador = "Administrador";
}

public class AuditableEntities
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
}

#region Fase 1 - Admin insere produto

public class Usuario : AuditableEntities
{
    protected Usuario()
    {
    }

    public Usuario(string nome, string tipo)
    {
        Nome = nome;
        Tipo = tipo;
    }
    
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

public class Categoria : AuditableEntities
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

public class Produto : AuditableEntities
{
    protected Produto()
    {
            
    }

    public Produto(Categoria categoria, string nome, decimal compraPorUnidade, decimal mercadoPorUnidade)
    {
        Categoria = categoria;
        Nome = nome;
        CompraPorUnidade = compraPorUnidade;
        MercadoPorUnidade = mercadoPorUnidade;
    }
    public Categoria Categoria { get; private set; }
    public string Nome { get; private set; }
    public decimal CompraPorUnidade { get; private set; }
    public decimal MercadoPorUnidade { get; private set; }
}

#endregion

#region Fase 2 - Cliente adicionar itens no carrinho

public class Item : AuditableEntities
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

public class Compra : AuditableEntities
{
    public List<Item> Itens { get; set; }
}

#endregion

#region Fase 3 - Admin - Fechou a onde

public class PreCompra : AuditableEntities
{
    protected PreCompra()
    {
        
    }

    public PreCompra(List<Compra> carrinhos, int quantidadePorPacote)
    {
        Carrinhos = carrinhos;
        QuantidadePorPacote = quantidadePorPacote;
    }
    public List<Compra> Carrinhos { get; private set; }
    public int QuantidadePorPacote { get; private set; }

    public int NumeroDePacotes
    {
        get
        {
            var total = Carrinhos.Sum(y => y.Itens.Sum(x => x.Quantidade));
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

public class Abastecimento : AuditableEntities
{    protected Abastecimento()
    {
        
    }

    public Abastecimento(Produto produto, int quantidade, decimal valorCompra, decimal valorMercado, decimal frete)
    {
        Produto = produto;
        Quantidade = quantidade;
        ValorCompra = valorCompra;
        ValorMercado = valorMercado;
        Frete = frete;
    }
    public Produto Produto { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorCompra { get; private set; }
    public decimal ValorMercado { get; private set; }
    public decimal Frete { get; private set; }
}

public class Onda : AuditableEntities
{
    protected Onda()
    {
        Compras = new List<Compra>();
    }

    public Onda(Abastecimento compraAdministrador, List<Compra> compras)
    {
        CompraAdministrador = compraAdministrador;
        Compras = compras;
    }
    public Abastecimento CompraAdministrador { get; private set; }
    public List<Compra> Compras { get; private set; }
    public decimal UltimoValorCadastrado => this.Compras.Sum(x => x.Itens.Sum(y => y.Produto.CompraPorUnidade));

    public decimal ValorPorUnidadeKg =>
        this.CompraAdministrador.ValorCompra / this.Compras.Sum(x => x.Itens.Sum(y => y.Quantidade));

    public decimal ValorDeCompra => this.Compras.Sum(x => x.Itens.Sum(y => y.Estimado));
    public decimal ValorDeMercadoPorUnidadeKg => this.CompraAdministrador.ValorMercado;

    public decimal ValorTotalMercado =>
        this.ValorDeMercadoPorUnidadeKg * this.Compras.Sum(x => x.Itens.Sum(y => y.Quantidade));

    public decimal Economia => ValorTotalMercado - ValorDeCompra;
    public decimal Frete => CompraAdministrador.Frete;
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

public class HistoricoPedido : AuditableEntities
{
    protected HistoricoPedido()
    {
        
    }

    public HistoricoPedido(Produto produto, decimal valorCompraUnidade, decimal valorMercadoUnidade)
    {
        Produto = produto;
        ValorCompraUnidade = valorCompraUnidade;
        ValorMercadoUnidade = valorMercadoUnidade;
    }
    public Produto Produto { get; set; }
    public decimal ValorCompraUnidade { get; set; }
    public decimal ValorMercadoUnidade { get; set; }
}

#endregion