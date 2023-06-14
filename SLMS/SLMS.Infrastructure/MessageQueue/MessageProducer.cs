using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Threading.Channels;

namespace SLMS.Infrastructure.MessageQueue
{
    public class MessageProducer : IDisposable
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly IModel _channel;

        public MessageProducer(IOptions<RabbitMQOptions> options)
        {
            _rabbitMQClient = new RabbitMQClient(options);
            _channel = _rabbitMQClient.CreateChannel();
        }

        public void Publish(string queuestr, byte[] body)
        {
            var queue = _channel.QueueDeclare(queuestr, false, false, false, null);
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
