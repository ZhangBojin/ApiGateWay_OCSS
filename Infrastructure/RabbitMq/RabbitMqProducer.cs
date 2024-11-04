using System.Text;
using RabbitMQ.Client;

namespace ApiGateWay_OCSS.Infrastructure.RabbitMq
{
    public class RabbitMqProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqProducer(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetSection("RabbitMq")["HostName"],
                Port = Convert.ToInt32(configuration.GetSection("RabbitMq")["Port"])
            }; 
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "Logs",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void SendLog(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "",
                routingKey: "logs",
                basicProperties: null,
                body: body);
        }
    }
}
