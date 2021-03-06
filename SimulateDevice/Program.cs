﻿using System;
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
        static DeviceClient deviceClient;
        static string iotHubUri = "{Escreva aqui a URI do IoTHub criado. EX: xxx.azure-devices.net}";
        static string deviceKey = "{Coloque aqui o deviceKey. EX: e5hJw/DasdasxVIrXRr3NMu8eFR78NH0bKtc=}";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("{Coloque aqui o nome do DeviceID}", deviceKey));
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
                        deviceId = "{Nome do deviceID}",
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
