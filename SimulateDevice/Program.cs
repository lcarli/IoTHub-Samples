using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;

namespace SimulateDevice
{
    class Program
    {
        //HostName=TrainLucas1.azure-devices.net;DeviceId=IntelEdison1;SharedAccessKey=e5hJ9Hl/DxVIrXRr3NMu8eFFUzQ9Jy7DxR78NH0bKtc=
        static DeviceClient deviceClient;
        static string iotHubUri = "TrainLucas1.azure-devices.net";
        static string deviceKey = "e5hJ9Hl/DxVIrXRr3NMu8eFFUzQ9Jy7DxR78NH0bKtc=";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("IntelEdison1", deviceKey));
            SendDeviceToCloudMessagesAsync();
            ReceiveC2dAsync();
            Console.ReadLine();
        }

            private static async void SendDeviceToCloudMessagesAsync()
            {
                double avgWindSpeed = 10; // m/s
                Random rand = new Random();

                while (true)
                {
                    double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                    var telemetryDataPoint = new
                    {
                        deviceId = "IntelEdison1",
                        windSpeed = currentWindSpeed
                    };
                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));

                    await deviceClient.SendEventAsync(message);
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                    Thread.Sleep(1000);
                }
            }

            private static async void ReceiveC2dAsync()
            {
                Console.WriteLine("\nReceiving cloud to device messages from service");
                while (true)
                {
                    Message receivedMessage = await deviceClient.ReceiveAsync();
                    if (receivedMessage == null) continue;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                    Console.ResetColor();

                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
    }
}
