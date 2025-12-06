using Microsoft.Extensions.DependencyInjection;

namespace Common.RabbitMQ;
public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMQServices
        (this IServiceCollection services)
    {
        services.AddScoped<IQueueService, QueueService>();
        services.AddScoped<IQueueConsumerService, QueueConsumerService>();

        return services;
    }
}
