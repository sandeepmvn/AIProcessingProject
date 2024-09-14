using System;
using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Dapper;
using System.Text;

namespace ProcessingFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("myqueue-items", Connection = "QueueConnectionString")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
            //process the message
            var proccesedMessage= JsonConvert.DeserializeObject<AnalysisModel>(message.MessageText);
            _logger.LogInformation($"C# Queue trigger function processed: {proccesedMessage}");

            //proccess the image

            string endpoint = Environment.GetEnvironmentVariable("VISION_ENDPOINT");
            string key = Environment.GetEnvironmentVariable("VISION_KEY");

            // Create an Image Analysis client.
            ImageAnalysisClient client = new ImageAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

            // Get the smart-cropped thumbnails for the image.
            ImageAnalysisResult result = await client.AnalyzeAsync(
                new Uri(proccesedMessage.ImageUrl),
                VisualFeatures.SmartCrops | VisualFeatures.DenseCaptions,
                new ImageAnalysisOptions { GenderNeutralCaption = true, SmartCropsAspectRatios = new float[] { 0.9F, 1.33F } });

            // Print smart-crops analysis results to the console
            StringBuilder stringBuilder = new StringBuilder();
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" SmartCrops:");
            foreach (CropRegion cropRegion in result.SmartCrops.Values)
            {
                stringBuilder.AppendLine($"   Aspect ratio: {cropRegion.AspectRatio}, Bounding box: {cropRegion.BoundingBox}");
                Console.WriteLine($"   Aspect ratio: {cropRegion.AspectRatio}, Bounding box: {cropRegion.BoundingBox}");
            }

            Console.WriteLine($" Dense Captions:");
            foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
            {
                stringBuilder.AppendLine($"   Region: '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
                Console.WriteLine($"   Region: '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
            }


            //Cropping
            //            SKBitmap originalImage = SKBitmap.Decode(stream);

            //            using var pixmap = new SKPixmap(originalImage.Info, originalImage.GetPixels());
            //            //SKRectI rectI = new SKRectI(30, 0, 574, 638);
            //            SKRectI rectI = new SKRectI(0, 0, 256, 256);

            //            var subset = pixmap.ExtractSubset(rectI);

            //            using var data = subset.Encode(SKPngEncoderOptions.Default);
            //            //File.WriteAllBytes(@$"Cropped{DateTime.Now}.png", data.ToArray());
            //            return data.ToArray();
            // once the message is processed, update the changes in database
            //update data in database
            proccesedMessage.IsProcessed = true;
            proccesedMessage.Result = stringBuilder.ToString();
            await UpdateProcessing(proccesedMessage);

        }


        static async Task UpdateProcessing(AnalysisModel model)
        {
            using (var connection = new SqlConnection("Server=localhost;Database=Processingdb;Trusted_Connection=True;"))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync("Update AnalysisProjecttbl set IsProcessed=@IsProcessed,Result=@Result where PKId=@PKId", model);
            }
        }
    }


    public class AnalysisModel
    {
        public string PKId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool IsProcessed { get; set; }
        public string Result { get; set; }
    }
}
