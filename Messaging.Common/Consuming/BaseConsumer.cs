using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
namespace Messaging.Common.Consuming
{
    public abstract class BaseConsumer<T>
    {
        private readonly IModel _channel;
        private readonly string _queue;

        protected BaseConsumer(IModel channel, string queue)
        {
            _channel = channel;
            _queue = queue;
        }

        public void Start()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<T>(body);

                    await HandleMessage(message!, ea.BasicProperties.CorrelationId);

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to process message: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);
        }

        protected abstract Task HandleMessage(T message, string correlationId);
    }
}
