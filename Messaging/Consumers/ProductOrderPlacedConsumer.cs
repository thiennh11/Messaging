using Messaging.Common.Consuming;
using Messaging.Common.Events;
using RabbitMQ.Client;

namespace Messaging.Consumers
{
    public class ProductOrderPlacedConsumer : BaseConsumer<OrderPlacedEvent>
    {
        public ProductOrderPlacedConsumer(IModel channel)
            : base(channel, "product.order_placed")
        {
        }

        protected override Task HandleMessage(OrderPlacedEvent message, string correlationId)
        {
            Console.WriteLine("=== Product Consumer Received Message ===");
            Console.WriteLine($"OrderNumber: {message.OrderNumber}");
            Console.WriteLine($"CustomerName: {message.CustomerName}");
            Console.WriteLine($"CustomerEmail: {message.CustomerEmail}");
            Console.WriteLine($"TotalAmount: {message.TotalAmount}");
            Console.WriteLine($"CorrelationId: {correlationId}");
            Console.WriteLine("========================================");

            return Task.CompletedTask;
        }
    }
}
