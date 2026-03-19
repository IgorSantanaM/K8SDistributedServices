using CommandService.EventProcessing;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor, IConnection connection, IChannel channel, string queueName) : BackgroundService
    {
        private async Task InitializeRabbitMq()
        {
            var factory = new ConnectionFactory() 
            { HostName = configuration["RabbitMQHost"]!, 
                Port = int.Parse(configuration["RabbitMQPort"]!) };

            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

            queueName = channel.QueueDeclareAsync().Result.QueueName;
            await channel.QueueBindAsync(queue: queueName, exchange: "trigger", routingKey: "");

            connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("Connection Shut down");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                await eventProcessor.ProcessEventAsync(notificationMessage, stoppingToken);
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }

        public override void Dispose()
        {
            if(channel.IsOpen)
            {
                channel.CloseAsync();
                connection.CloseAsync();
            }
            base.Dispose();
        }
    }
}
