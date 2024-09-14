using Azure.Storage.Queues;

namespace WebAppProcessingExample.Services
{
    public class QueueService
    {
        QueueClient queueClient = null;
        public QueueService()
        {

            // Instantiate a QueueClient to create and interact with the queue
            queueClient = new QueueClient("DefaultEndpointsProtocol=https;AccountName=14092024imgprocessing;AccountKey=CeGiiAVV7Sf51Fnr1UQ923uNSjMNjbV4ilp+AVct9Jtwa5DE4cF3fpuTTSTGJCyTa0F0yC4tXj03+ASteDC1nA==;EndpointSuffix=core.windows.net", "myqueue-items");
        }

        public async Task SendMessageAsync(string message)
        {
            // Send a message to the queue
            await queueClient.SendMessageAsync(Base64Encode(message));
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
