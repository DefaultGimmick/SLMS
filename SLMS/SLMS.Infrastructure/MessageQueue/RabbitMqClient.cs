using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace SLMS.Infrastructure.MessageQueue
{
    public class RabbitMqClient
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        public RabbitMqClient(IOptions<RabbitMqOptions> msg)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = msg.Value.HostName,
                UserName = msg.Value.UserName,
                Password = msg.Value.Password,
                VirtualHost = msg.Value.VirtualHost
            };
        }

        public IModel CreateChannel()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = _connectionFactory.CreateConnection();
            }

            return _connection.CreateModel();
        }
    }
}
