using Microsoft.EntityFrameworkCore.Metadata;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;   
        private readonly IChannel _channel;
        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]!)};

            try
            {

                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;  
            }
            catch (Exception ex)
            {
            }
        }
        public async Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                await SendMessageAsync(message);   
            }
            else
            {
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(exchange: "trigger",
                                routingKey: "", 
                                body: body);
        }

        private async Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("Connection is being shut");
        }
        public async ValueTask DisposeAsync()
        {
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
        }
    }
}