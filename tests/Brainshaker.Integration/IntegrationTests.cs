using System.Text.Json;
using Brainshaker.App.Commons;
using Brainshaker.App.UseCases.GetUser;
using Brainshaker.Domain.Entities;
using Brainshaker.Domain.Repositories;
using Brainshaker.Infra.Database;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Brainshaker.Integration;

public class IntegrationTests
{
    [Fact]
    public void Test1()
    {
    }
}

public class ResidentControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private MsSqlContainer _container;

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder()
            .WithName(Guid.NewGuid().ToString())
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPassword("1q2w3e4r@#$!")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();

        await _container.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
                    if (descriptor is not null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<DatabaseContext>(options =>
                    {
                        var connectionString = _container.GetConnectionString();
                        options.UseSqlServer(connectionString);
                    });
                });
            });

        // Initialize database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }

    [Fact]
    public async Task Given_GetUsuarioById_When_UsuarioNotFound_Should_ReturnsNotFoundResult()
    {
        var client = _factory.CreateClient();
        var id = 1;
        var response = await client.GetAsync($"/api/usuarios/{id}");
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllResidents_ReturnsEmptyList()
    {
        var client = _factory.CreateClient();
        var repository = _factory.Services.GetRequiredService<IRepository<Usuario>>();
        var usuario = new Usuario("nome", "tipo");
        await repository.AddAsync(usuario);
        var response = await client.GetAsync($"/api/usuarios/{usuario.Id}");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var allresidentsResponse = JsonSerializer.Deserialize<Usuario>(content,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        allresidentsResponse.Should().NotBeNull();
        allresidentsResponse.Nome.Should().Be("Nome");
        allresidentsResponse.Tipo.Should().Be("Tipo");
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
        _factory.Dispose();
    }
}