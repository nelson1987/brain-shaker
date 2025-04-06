using Brainshaker.Domain.Entities;

namespace Brainshaker.Unit;

/// <summary>
/// RF 1 -
/// Eu, apenas como administrador
/// Quero cadastrar uma categoria
/// RN 1 - A categoria deve ter um nome
/// RN 2 - A categoria deve ter um nome que não se repete em toda a base
///
/// RF 2 -
/// Eu, como cliente, não posso cadastrar uma categoria
/// </summary>
public class CategoriaUnitTests
{
    [Fact]
    public void Dado_CadastrarCategoria_Quando_DadosValido_Deve_RetornarCategoria()
    {
        Categoria fruta = new Categoria("Fruta");
        Assert.Equal("Fruta", fruta.Nome);
    }

    [Fact]
    public void Dado_CadastrarCategoria_Quando_CadatradoPorCliente_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Dado_CadastrarCategoria_Quando_NomeExistente_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Dado_CadastrarCategoria_Quando_NaoTiverUmNome_Deve_DispararExcecao()
    {
        throw new NotImplementedException();
    }
}