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