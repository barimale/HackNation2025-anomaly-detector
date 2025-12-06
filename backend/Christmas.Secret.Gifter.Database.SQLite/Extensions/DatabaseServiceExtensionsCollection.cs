using Microsoft.Extensions.DependencyInjection;
using MSSql.Infrastructure.Repositories;
using MSSql.Infrastructure.Repositories.Abstractions;

namespace MSSql.Infrastructure.Extensions {
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddSQLLiteDatabase(this IServiceCollection services)
        {
            //services.AddTransient<IAlgorithmResultRepository, AlgorithmResultRepository>();
            //services.AddTransient<IAlgorithmSummaryRepository, AlgorithmSummaryRepository>();

            ////WIP check it
            services.AddSQLLiteDatabaseAutoMapper();

            return services;
        }

        public static IServiceCollection AddSQLLiteDatabaseAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Mappings));

            return services;
        }
    }
}
