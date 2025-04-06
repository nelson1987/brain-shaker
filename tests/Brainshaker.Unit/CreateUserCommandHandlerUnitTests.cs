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

    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerUnitTests()
    {
        _handler = _fixture.Create<CreateUserCommandHandler>();
    }

    [Fact]
    public async Task Dado_HandleAsync_Quando_UsuarioValido_Deve_Retornar_Dado()
    {
        var command = new CreateUserCommand("Nome de Usuario");
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Dado_HandleAsync_Quando_NomeInvalido_Deve_Retornar_NomeInvalido()
    {
        var command = new CreateUserCommand(string.Empty);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(result.Error, CreateUserCommandErrors.UsernameRequired);
    }

    [Fact]
    public async Task Dado_HandleAsync_Quando_NomeMenorQue6_Deve_Retornar_NomeMenorQue6()
    {
        var command = new CreateUserCommand("Nome");
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(result.Error, CreateUserCommandErrors.UsernameTooShort);
    }
}