using System.Text;
using ApiGateWay_OCSS.Domain;
using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using ApiGateWay_OCSS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ApiGateWay_OCSS.Infrastructure.RabbitMq
{
    public class RabbitMqProducer
    {
        private readonly IModel _channel;

        public RabbitMqProducer(IConfiguration configuration, LogServiceDbContext logServiceDbContext)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetSection("RabbitMq")["HostName"],
                Port = Convert.ToInt32(configuration.GetSection("RabbitMq")["Port"]),
                UserName = configuration.GetSection("RabbitMq")["Username"],
                Password = configuration.GetSection("RabbitMq")["Password"]
            }; 
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: "Logs",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
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
                UserId=userInfo.Id,
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
