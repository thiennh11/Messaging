using Messaging.Common.Events;
using Messaging.Common.Options;
using Messaging.Common.Publishing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Messaging.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly Publisher _publisher;
        private readonly RabbitMqOptions _rabbitOptions;

        public OrdersController(Publisher publisher, IOptions<RabbitMqOptions> rabbitOptions)
        {
            _publisher = publisher;
            _rabbitOptions = rabbitOptions.Value;
        }

        [HttpPost("place")]
        public IActionResult PlaceOrder()
        {
            var orderEvent = new OrderPlacedEvent
            {
                OrderId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                OrderNumber = "ORD-001",
                CustomerName = "Nguyen Van A",
                CustomerEmail = "vana@example.com",
                PhoneNumber = "0123456789",
                TotalAmount = 500000,
                Items = new List<OrderItemLine>
                {
                    new OrderItemLine
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 2,
                        UnitPrice = 250000
                    }
                },
                CorrelationId = Guid.NewGuid().ToString()
            };

            _publisher.Publish(
                exchange: _rabbitOptions.ExchangeName,
                routingKey: "order.placed",
                message: orderEvent,
                correlationId: orderEvent.CorrelationId);

            return Ok(new
            {
                Message = "OrderPlacedEvent published successfully"
            });
        }
    }
}