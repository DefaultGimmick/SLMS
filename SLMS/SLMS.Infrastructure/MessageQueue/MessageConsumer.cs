using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Channels;
using Newtonsoft.Json;
using SLMS.Models.Dtos.Book;
using SLMS.Application.Books;
using SLMS.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace SLMS.Infrastructure.MessageQueue
{
    public class MessageConsumer : BackgroundService
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public MessageConsumer(IOptions<RabbitMQOptions> options, IServiceProvider serviceProvider)
        {
            _rabbitMQClient = new RabbitMQClient(options);
            _channel = _rabbitMQClient.CreateChannel();
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //定义队列
            var queue = _channel.QueueDeclare("BookStorage", false, false, false, null);
            _channel.BasicQos(0, 1, false);
            // 定义消费者
            var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var result = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                        EntityBook u = (EntityBook)JsonConvert.DeserializeObject(result, typeof(EntityBook));
                        var _bookAppService = scope.ServiceProvider.GetRequiredService<IBookAppService>();
                        await _bookAppService.AddBookAsync(u);
                        _channel.BasicAck(e.DeliveryTag, false);
                        Console.WriteLine("消费消息：" + result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("消费者错误：" + ex.Message);
                }
            };
            _channel.BasicConsume(queue.QueueName, false, "UpdateStorage", false, false, null, consumer);
            // 等待取消标记以停止后台服务
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void  Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}
