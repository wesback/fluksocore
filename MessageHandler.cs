using System;
using System.Text;
using MQTTnet.Client;
using MQTTnet;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;

namespace FluksoCore
{
    public class MessageHandler
    {
        private static EventHubClient eventHubClient;
        
        static MessageHandler()
        {
            //Open EventHubs connection
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(FluksoConfig.getConfig("EventHubs:EHConnectionString"))
            {
                EntityPath = FluksoConfig.getConfig("EventHubs:EHEntityPath")
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            //Close the connection gracefully in case program gets stopped
            AppDomain.CurrentDomain.ProcessExit += MessageHandler_Close;

        }

        static async void MessageHandler_Close(object sender, EventArgs e) 
        {
            Console.WriteLine("Closing connection");
            await eventHubClient.CloseAsync();
        }

        public static async void ConsoleHandler(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            SensorData sensorData =  await ParseMessage(e.ApplicationMessage.Payload, e.ApplicationMessage.Topic);

            if ((sensorData.type == "W" || sensorData.type == "L/s") && sensorData.value > 0 )
            {
               Console.WriteLine ($"{sensorData.Name} | {sensorData.TopicName} | {sensorData.timeStamp} - {sensorData.type} - {sensorData.value}");
            }
        } 

        public static async void EventHubsHandler(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            SensorData sensorData =  await ParseMessage(e.ApplicationMessage.Payload, e.ApplicationMessage.Topic);
            
            if ((sensorData.type == "W" || sensorData.type == "L/s") && sensorData.value > 0 )
            {
                //Send message
                await SendMessagesToEventHub(sensorData);
            }
        } 

        private static async Task SendMessagesToEventHub(SensorData sensorData)
        {
            try
            {
                var sensorDataMessage = JsonConvert.SerializeObject(sensorData);
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(sensorDataMessage)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }

        private static async Task<SensorData> ParseMessage(byte[] message, string topicName)
        {
            CultureInfo cultureInfo = new CultureInfo("en-us");
            var msg = Encoding.UTF8.GetString(message);
            var splitMsg = msg.Split(",");            
            
            SensorData sensorData = new SensorData();
            sensorData.TopicName = topicName;
            sensorData.epochTime = long.Parse(splitMsg[0].Remove(0,1));
            sensorData.value = double.Parse(splitMsg[1].Trim(), cultureInfo);
            sensorData.type = splitMsg[2].Substring(2, splitMsg[2].Trim().LastIndexOf("]")-2).Trim();
            
            return await Task.FromResult(sensorData);
        }
    }
}