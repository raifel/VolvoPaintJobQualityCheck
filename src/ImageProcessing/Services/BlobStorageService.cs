using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageProcessing.Services
{
    public class BlobStorageService
    {

        /// <summary>
        /// Reads the content of the image.
        /// </summary>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="containerReferenceName">Name of the container reference.</param>
        /// <returns></returns>
        public Bitmap ReadImageContent(string blobName, string containerReferenceName)
        {
            var connectionString = CloudConfigurationManager.GetSetting("imageprocessiong3_AzureStorageConnectionString");

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);


            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("containerReferenceName");

       
            // Retrieve reference to a blob
            CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(blobName);
            Bitmap bitmapImage;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob2.DownloadToStream(memoryStream);
                bitmapImage= new Bitmap(memoryStream);
            }
            return bitmapImage;
        }
    }
}