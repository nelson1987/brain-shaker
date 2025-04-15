// using System.Security.Claims;
// using System.Text.Encodings.Web;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.EntityFrameworkCore.Storage;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace IntegrationTests;
//
// public class UnitTest1
// {
//     [Fact]
//     public void Test1()
//     {
//     }
// }
//
// public class AssetsManagerApi : WebApplicationFactory<Program>
// {
//     static AssetsManagerApi()
//         => Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
//
//     protected override void ConfigureWebHost(IWebHostBuilder builder)
//         => builder.UseEnvironment("Test")
//             .ConfigureTestServices(services =>
//             {
//                 services.AddAuthentication(defaultScheme: "TestScheme")
//                     .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
//
//                 //KafkaFixture.ConfigureKafkaServices(services);
//                 
//             });
// }
//
// public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
// {
//     public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
//         ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
//         : base(options, logger, encoder, clock)
//     {
//     }
//
//     protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//     {
//         var claims = new[]
//         {
//             new Claim(ClaimTypes.Name, "Test user"),
//             new Claim("preferred_username", "user@email.com.br")
//         };
//         var identity = new ClaimsIdentity(claims, "Test");
//         var principal = new ClaimsPrincipal(identity);
//         var ticket = new AuthenticationTicket(principal, "TestScheme");
//
//         var result = AuthenticateResult.Success(ticket);
//
//         return Task.FromResult(result);
//     }
// }
// /*
// public static class KafkaFixture
// {
//     private static readonly IEventClient? EventClient;
//     public static void ConfigureKafkaServices(IServiceCollection services)
//         => services
//             //.RemoveAll<KafkaHealthCheck>()
//             .RemoveAll<IEventClient>()
//             //.RemoveAll<IEventClientConsumers>()
//             //.RemoveAll<IEventClientProducer>()
//             .AddSingleton(_ => EventClient);
//     //.AddSingleton<IEventClientConsumers>(_ => EventClientConsumers)
//     //.AddSingleton<IEventClientProducer>(_ => EventClientProducer);
//     public static Task Produce<T>(T data)
//     {
//         using var cancellationToken = ExpiringCancellationToken();
//         AddTeacherCreatedClientEvent? evento = data as AddTeacherCreatedClientEvent;
//         return EventClient.Produce(
//                 evento,
//                 cancellationToken.Token);
//     }
//     public static T Consume<T>(string topic, int timeout = 150)
//     {
//         try
//         {
//             using var cancellationToken = ExpiringCancellationToken(timeout);
//             return EventClient.Consume(topic, JsonSerializer.Deserialize<T>, cancellationToken.Token);
//         }
//         catch { return default!; }
//     }
//     private static CancellationTokenSource ExpiringCancellationToken(int msTimeout = 150)
//     {
//         var timeout = TimeSpan.FromMilliseconds(msTimeout);
//         return new CancellationTokenSource(timeout);
//     }
// }
// public interface IEventClient
// {
//     T Consume<T>(string topicName, Func<Stream, JsonSerializerOptions?, T?> deserialize, CancellationToken token = default);
//     Task Produce(AddTeacherCreatedClientEvent? addTeacherCreatedClientEvent, CancellationToken token);
// }
// public static class PostgresqlFixture
// {
//     public static DatabaseContext? Context { get; private set; }
//
//     static PostgresqlFixture() { Context = new DatabaseContext(); }
// }
// public static class BrokerFixture
// {
//     public static Broker? Broker { get; private set; }
//
//     static BrokerFixture() { Broker = new Broker(); }
// }
// */