namespace Common.RabbitMQ {
    public interface IQueueService
    {
        Task Publish(string message, string channelName);
        Task Publish<T>(T message, string channelName);
    }
}
