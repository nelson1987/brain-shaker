using AutoFixture;
using AutoFixture.AutoMoq;
using Brainshaker.App.UseCases.CreateUser;

namespace Brainshaker.Unit;

public class CreateUserCommandHandlerUnitTests
{
    private readonly IFixture _fixture = new Fixture()
        .Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true
        });

    private readonly CreateUsuarioCommandHandler _handler;

    public CreateUserCommandHandlerUnitTests()
    {
        _handler = _fixture.Create<CreateUsuarioCommandHandler>();
    }

    [Fact]
    public async Task Dado_HandleAsync_Quando_UsuarioValido_Deve_Retornar_Dado()
    {
        var command = new CreateUsuarioCommand("Nome de Usuario");
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Dado_HandleAsync_Quando_NomeInvalido_Deve_Retornar_NomeInvalido()
    {
        var command = new CreateUsuarioCommand(string.Empty);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(result.Error, CreateUsuarioCommandErrors.UsernameRequired);
    }

    [Fact]
    public async Task Dado_HandleAsync_Quando_NomeMenorQue6_Deve_Retornar_NomeMenorQue6()
    {
        var command = new CreateUsuarioCommand("Nome");
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(result.Error, CreateUsuarioCommandErrors.UsernameTooShort);
    }
}

/*
 * [POST] api/categorias
 * {
 *  "CategoriaId" : 1,
 *  "Nome": "Banana",
 *  "ValorCompra": 4.00,
 *  "ValorMercado": 7.00
 * }
 */

#region Fase 1 - Admin insere produto

// [Admin] Cadastrar Categoria
// [Admin] Cadastrar Produto
// [Aplicacao] Cadastrar Usuario

#endregion

#region Fase 2 - Cliente adicionar itens no carrinho

#endregion

#region Fase 3 - Admin - Fechou a onde

#endregion

#region Fase 4 - Criacao Historico

#endregion

// [Admin] Cadastrar Categoria
// [Admin] Cadastrar Produto
// [Aplicacao] Iniciar a Onda
// [Cliente] Cadastrar Compra
// [Aplicacao] Fechar a Onda
// [Admin] Visualizar Pre Compra
// [Admin] Cadastrar Abastecimento
// [Aplicacao] Notificar Cliente
// [Cliente] Pagar Compra