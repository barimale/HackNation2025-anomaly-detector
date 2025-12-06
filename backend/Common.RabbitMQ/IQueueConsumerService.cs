using RabbitMQ.Client.Events;

namespace Common.RabbitMQ {
    public interface IQueueConsumerService
    {
        Task StartAsync(AsyncEventHandler<BasicDeliverEventArgs> body, string channelName, CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
