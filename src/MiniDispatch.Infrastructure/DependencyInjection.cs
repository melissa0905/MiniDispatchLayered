using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Infrastructure.Persistence;
using MiniDispatch.Infrastructure.Repositories;

namespace MiniDispatch.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
        {
            services.AddDbContext<DispatchDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "dispatch");
                });
            });

            services.AddScoped<
                IShipmentRepository,
                ShipmentRepository>();

            return services;
        }

    }
}