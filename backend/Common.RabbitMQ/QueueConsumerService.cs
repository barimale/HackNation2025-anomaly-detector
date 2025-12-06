using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.RabbitMQ {
    public class QueueConsumerService : IQueueConsumerService
    {
        private IConnection _connection;
        private IChannel _channel;

        private AsyncEventingBasicConsumer _consumer;
        private ConnectionFactory factory;

        private readonly ILogger<QueueConsumerService> _logger;
        public QueueConsumerService(
            ILogger<QueueConsumerService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            var hostname = configuration.GetSection("RabbitMQ:HostName").Value;
            ushort consumerDispatchConcurrency = ushort.Parse(configuration.GetSection("RabbitMQ:ConsumerDispatchConcurrency").Value);
            factory = new ConnectionFactory() { HostName = hostname, ConsumerDispatchConcurrency = consumerDispatchConcurrency };
        }

        public async Task StartAsync(AsyncEventHandler<BasicDeliverEventArgs> body, string channelName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service running.");

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: channelName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
            arguments: null);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += body;

            await _channel.BasicConsumeAsync(
                channelName,
                autoAck: true,
                consumer: _consumer);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
