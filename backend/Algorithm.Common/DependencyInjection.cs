using Algorithm.Common.ML;
using Microsoft.Extensions.DependencyInjection;

namespace Common.RabbitMQ;
public static class DependencyInjection
{
    public static IServiceCollection AddAlgorithmCommonServices
        (this IServiceCollection services)
    {
        services.AddScoped<ICustomMlContext, CustomMlContext>();

        return services;
    }
}
