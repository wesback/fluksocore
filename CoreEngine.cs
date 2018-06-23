using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using MQTTnet.Client;
using MQTTnet;
using System.Text;
using MQTTnet.ManagedClient;

namespace FluksoCore
{
    class CoreEngine
    {
        public static async Task GetMessages()
        {
            var fluksoIP = FluksoConfig.getConfig("FluksoServer:FluksoIP");

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("FluksoCore")
                    .WithTcpServer(fluksoIP)
                    .Build())
                .Build();

            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("/sensor/+/+").Build());
            await mqttClient.StartAsync(options);

            Console.WriteLine("Starting FluksoCore");
            Console.WriteLine($"Listening on {fluksoIP}");
                        
            if (FluksoConfig.getConfig("Output:Console") == "true")
            {
                mqttClient.ApplicationMessageReceived += MessageHandler.ConsoleHandler;
                Console.WriteLine("Output Console enabled");
            }
            if (FluksoConfig.getConfig("Output:EventHubs") == "true")
            {
                mqttClient.ApplicationMessageReceived += MessageHandler.EventHubsHandler;
                Console.WriteLine("Output EventHubs enabled");
            }

            await Task.Delay(Timeout.Infinite);
       }
    }
}