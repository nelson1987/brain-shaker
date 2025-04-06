using Brainshaker.Infra.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brainshaker.Infra;

public static class Dependencies
{
    public static void AddInfra(this IServiceCollection service,
        IConfiguration configuration)
    {
        service.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"
            ), b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
    }
}