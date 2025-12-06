using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Common.RabbitMQ {
    public class QueueService : IQueueService
    {
        private readonly ILogger<QueueService> _logger;
        private readonly ConnectionFactory _factory;

        public QueueService(
            ILogger<QueueService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            var hostname = configuration.GetSection("RabbitMQ:HostName").Value;
            _factory = new ConnectionFactory() { HostName = hostname };
        }

        public async Task Publish(string message, string channelName)
        {
            using var _connection = await _factory.CreateConnectionAsync();
            using var _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: channelName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: channelName,
                Encoding.UTF8.GetBytes(message));
        }

        public async Task Publish<T>(T message, string channelName) {
            using var _connection = await _factory.CreateConnectionAsync();
            using var _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: channelName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            var serialized = JsonSerializer.Serialize(message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: channelName,
                Encoding.UTF8.GetBytes(serialized));
        }

    }
}
