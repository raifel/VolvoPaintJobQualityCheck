using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Volov.PaintJobCheck.ImageProcessingFuntion
{
    public static class ProcessPaintJobImages
    {

       public static string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=paintcheckimages;AccountKey=qhuiRPu8Dr0ZKQzz6f68I+H2kIX+TRlNKfUWQLZDJx64KJR+Nck143VjsTLESA9UMzZc1X+hFY033vTynXAMWg==;EndpointSuffix=core.windows.net";
        

        [FunctionName("ProcessPaintJobImages")]
        public static void Run([BlobTrigger("paintjobimages/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        }

        public static bool CanStartProcessing(string name)
        {

            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;


            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("paintjobimages");
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    cloudBlobContainer.SetPermissionsAsync(permissions);

                    
                    CloudBlockBlob blockBlob = directory.GetBlockBlobReference(myBlob);
                    await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }

            }
            else
            {

            }

        }
    }
}
