using Azure.Messaging.ServiceBus;

namespace SmartCity.MessageBroker
{
    public class ServiceBusSenderService
    {


        //implemet generic method
        public async Task SendMessageAsync(string messageBody)
        {
            var connectionString = "Endpoint=sb://***.servicebus.windows.net/;SharedAccessKeyName=***;SharedAccessKey=***";



           
            var queueName = "test-queue";
            // Create a client
            await using var client = new ServiceBusClient(connectionString);

            // Create a sender for the queue
            ServiceBusSender sender1 = client.CreateSender(queueName);

            // Create and send a message
            ServiceBusMessage message = new ServiceBusMessage("Hello, this is a test message!");
            ServiceBusMessage message1 = new ServiceBusMessage("Hello, this is a test message!1111");
            ServiceBusMessage message2 = new ServiceBusMessage("Hello, this is a test message!2222");
            ServiceBusMessage message3 = new ServiceBusMessage("Hello, this is a test message!33333");
            await sender1.SendMessageAsync(message);
            //await sender.SendMessageAsync(message1);
            //await sender.SendMessageAsync(message2);
            //await sender.SendMessageAsync(message3);

            Console.WriteLine("Message sent successfully!");

            // Dispose of the sender and client
            await sender1.DisposeAsync();

            var queueName2 = "test-queue2";
            ServiceBusSender sender2 = client.CreateSender(queueName2);
            await sender2.SendMessageAsync(message);

            await client.DisposeAsync();

        }
    }
}
