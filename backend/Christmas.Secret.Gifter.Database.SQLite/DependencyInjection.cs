using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSSql.Infrastructure.Repositories;
using MSSql.Infrastructure.Repositories.Abstractions;

namespace MSSql.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddMSSQLServices
        (this IServiceCollection services)
    {
        services.AddScoped<IAlgorithmResultRepository, AlgorithmResultRepository>();
        services.AddScoped<IAlgorithmSummaryRepository, AlgorithmSummaryRepository>();
        services.AddScoped<IRankingRepository, RankingRepository>();

        services.AddDbContextFactory<DataDbContext>(options =>
            options
                .UseSqlite("Data Source=R:/Application.db;Cache=Shared",
            b => b.MigrationsAssembly(typeof(DataDbContext).Assembly.FullName)));
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
