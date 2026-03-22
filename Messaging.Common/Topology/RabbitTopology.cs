using Messaging.Common.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Common.Topology
{
    public static class RabbitTopology
    {
        public static void EnsureAll(IModel channel, RabbitMqOptions rabbitMqOptions)
        {
            channel.ExchangeDeclare(
                exchange: rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            if (!string.IsNullOrWhiteSpace(rabbitMqOptions.DlxExchangeName))
            {
                channel.ExchangeDeclare(
                    exchange: rabbitMqOptions.DlxExchangeName!,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false);

                if (!string.IsNullOrWhiteSpace(rabbitMqOptions.DlxQueueName))
                {
                    channel.QueueDeclare(
                        queue: rabbitMqOptions.DlxQueueName!,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.QueueBind(
                        rabbitMqOptions.DlxQueueName,
                        rabbitMqOptions.DlxExchangeName!,
                        routingKey: "");
                }
            }

            var args = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(rabbitMqOptions.DlxExchangeName))
            {
                args["x-dead-letter-exchange"] = rabbitMqOptions.DlxExchangeName!;
            }

            channel.QueueDeclare(
                queue: rabbitMqOptions.ProductOrderPlacedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);

            channel.QueueDeclare(
                queue: rabbitMqOptions.NotificationOrderPlacedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);

            channel.QueueBind(
                queue: rabbitMqOptions.ProductOrderPlacedQueue,
                exchange: rabbitMqOptions.ExchangeName,
                routingKey: "order.placed");

            channel.QueueBind(
                queue: rabbitMqOptions.NotificationOrderPlacedQueue,
                exchange: rabbitMqOptions.ExchangeName,
                routingKey: "order.placed");
        }
    }
}
