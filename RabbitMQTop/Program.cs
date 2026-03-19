using RabbitMQ.Client;
using System.Text;

namespace RabbitMQTop
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.QueueDeclareAsync(queue: "pizzaqueue", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: "pizzaqueue", exchange: "logs", routingKey: string.Empty);
            await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body);
            Console.WriteLine($" [x] Sent {message}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            static string GetMessage(string[] args)
            {
                return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
            }
        }
    }
}
