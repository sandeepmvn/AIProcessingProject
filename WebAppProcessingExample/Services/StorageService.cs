using Azure.Identity;
using Azure.Storage.Blobs;

namespace WebAppProcessingExample.Services
{
    public class StorageService
    {
        BlobServiceClient blobServiceClient;
        public StorageService() {

            //blobServiceClient = new BlobServiceClient(
            //        new Uri("https://14092024imgprocessing.blob.core.windows.net"),
            //        new DefaultAzureCredential());

            blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=14092024imgprocessing;AccountKey=CeGiiAVV7Sf51Fnr1UQ923uNSjMNjbV4ilp+AVct9Jtwa5DE4cF3fpuTTSTGJCyTa0F0yC4tXj03+ASteDC1nA==;EndpointSuffix=core.windows.net");
        }


        //public async Task<string> CreateContainer(string containerName)
        //{
        //    //Create a unique name for the container
        //    ///string containerName = "quickstartblobs" + Guid.NewGuid().ToString();

        //    // Create the container and return a container client object
        //    //BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        //    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        //    await containerClient.CreateIfNotExistsAsync();
        //    return containerClient.Uri.AbsoluteUri;
        //}


        public async Task<string> UploadFile(string containerName, string fileName, Stream stream)
        {
            // Get a reference to a container named "sample-container" and then create it
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to a blob named "sample-file" in a container named "sample-container"
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload local file
            await blobClient.UploadAsync(stream, true);
            return blobClient.Uri.AbsoluteUri;
        }
    }
}
