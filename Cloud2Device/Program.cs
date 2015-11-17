using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Cloud2Device
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=TrainLucas1.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=PmHijohWzgco6GH86yp+qBEeXTD57p34BbpBgfyzwyY=";

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            ReceiveFeedbackAsync();
            string text = "";
            do
            {
                Console.Write("Write a message to send: ");
                text = Console.ReadLine();
                SendCloudToDeviceMessageAsync(text).Wait();
                Console.ReadLine();
            } while (text != "1");
        }

        private async static Task SendCloudToDeviceMessageAsync(string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync("IntelEdison1", commandMessage);
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}
