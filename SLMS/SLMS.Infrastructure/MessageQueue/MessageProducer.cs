using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace SLMS.Infrastructure.MessageQueue
{
    public class MessageProducer : IDisposable
    {
        private readonly IModel _channel;

        public MessageProducer(IOptions<RabbitMqOptions> options)
        {
            var rabbitMqClient = new RabbitMqClient(options);
            _channel = rabbitMqClient.CreateChannel();
        }

        public void Publish(string queueStr, byte[] body)
        {
            var queue = _channel.QueueDeclare(queueStr, false, false, false, null);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish("", queue.QueueName, false, properties, body);
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}
