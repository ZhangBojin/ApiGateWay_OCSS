using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using ApiGateWay_OCSS.Infrastructure.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ApiGateWay_OCSS.Infrastructure.RabbitMq
{
    public class RabbitMqProducer
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;

        public RabbitMqProducer(IConfiguration configuration, LogServiceDbContext logServiceDbContext)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetSection("RabbitMq")["HostName"],
                Port = Convert.ToInt32(configuration.GetSection("RabbitMq")["Port"]),
                UserName = configuration.GetSection("RabbitMq")["Username"],
                Password = configuration.GetSection("RabbitMq")["Password"]
            }; 
             _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "Logs",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        ~RabbitMqProducer()
        {
            _channel.Close();
            _connection.Close();
        }

        public void Log(UserInfo? userInfo,string controller,string action,string msg,string level)
        {
            var log = new Log()
            {
                CreatTime = DateTime.Now,
                Level= level,
                Msg= msg,
                Controller= controller,
                Action= action,
                UserId = userInfo!.Id,
                UserName=userInfo.Name,
                UserEmail=userInfo.Email,
            };
            var jsonMessage = JsonConvert.SerializeObject(log);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "",
                routingKey: "Logs",
                basicProperties: null,
                body: body);
        }
    }
}
