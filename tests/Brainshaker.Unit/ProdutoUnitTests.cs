using Brainshaker.Domain.Entities;

namespace Brainshaker.Unit;

/// <summary>
/// RF 1 -
/// Eu, apenas como administrador
/// Quero cadastrar um produto
/// RN 1 - A categoria do produto deve estar cadastrada
/// RN 2 - O produto deve ter um nome
/// RN 3 - O produto deve ter um nome que não se repete em toda a base
/// RN 4 - O produto deve ter um valor de Compra por Unidade
///
/// RF 2 -
/// Eu, como cliente, não posso cadastrar Produto
/// </summary>
public class ProdutoUnitTests
{
    [Fact]
    public void Dado_CadastrarProduto_Quando_DadosValidos_Deve_RetornarProduto()
    {
        Categoria fruta = new Categoria("Fruta");
        Produto banana = new Produto(Guid.NewGuid(), fruta, "Banana", 4.00M, 7.00M);
        Assert.Equal("Fruta", banana.Categoria.Nome);
        Assert.Equal("Banana", banana.Nome);
        Assert.Equal(4.00M, banana.CompraPorUnidade);
        Assert.Equal(7.00M, banana.MercadoPorUnidade);
    }

    [Fact]
    public void Dado_CadastrarProduto_Quando_UsuarioCliente_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Dado_CadastrarProduto_Quando_CategoriaInexistente_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Dado_CadastrarProduto_Quando_NomeInvalido_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Dado_CadastrarProduto_Quando_NomeExistente_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Dado_CadastrarProduto_Quando_ValorComprarUnidadeInvalido_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }
}