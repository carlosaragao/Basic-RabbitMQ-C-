using Orders.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace OrderWorker
{
    class Program
    {
        static void Main(string[] args)
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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var order = JsonSerializer.Deserialize<Order>(body);

                        Console.WriteLine($"Order Id: {order.Id} | {order.ItemName} | {order.Price:N2}");
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        // Logger
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                    
                };
                channel.BasicConsume(queue: "orderQueue",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
