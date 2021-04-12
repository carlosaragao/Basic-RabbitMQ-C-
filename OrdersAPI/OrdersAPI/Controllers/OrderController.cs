using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orders.Domain;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult InsertOrder(Order order)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "orderQueue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = JsonSerializer.Serialize(order);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "orderQueue",
                                         basicProperties: null,
                                         body: body);
                }

                return Accepted(order);
            }
            catch (Exception e)
            {
                _logger.LogError("Erro ao tentar criar um novo pedido", e);
                return new StatusCodeResult(500);
            }
        }
    }
}
