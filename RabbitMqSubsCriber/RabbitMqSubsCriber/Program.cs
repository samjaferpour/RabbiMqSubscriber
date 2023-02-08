using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory
{
    HostName= "localhost",
    VirtualHost= "/",
    UserName= "guest",
    Password= "guest",
    Port = 5672
};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "chat.exchange", type: ExchangeType.Direct, durable: true);
channel.QueueDeclare(queue: "chat_queue", durable: true, autoDelete: false, exclusive: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (ch, ea) =>
{
    var body = ea.Body.ToArray();
    var result = Encoding.UTF8.GetString(body);
    Console.WriteLine(result);
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
channel.BasicConsume(queue: "", autoAck: false, exclusive: false, consumer: consumer);
Console.ReadLine();