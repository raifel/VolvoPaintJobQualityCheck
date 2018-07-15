using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Volvo.VolvoPaintJobImageUpload.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Image")]
    public class ImageController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public ImageController(IHostingEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        private async void uploadBlob(IFormFile file,CloudBlobClient cloudBlobClient,string containername)
        {
            CloudBlobContainer cloudBlobContainer = null;
            cloudBlobContainer = cloudBlobClient.GetContainerReference(containername);
            await cloudBlobContainer.CreateIfNotExistsAsync();

            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync(permissions);
            BlobContinuationToken blobContinuationToken = null;
            var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
            if (results.Results.Count() < 2)
            {
                MemoryStream memoryStream = new MemoryStream();
                // file.CopyTo(memoryStream);
                MagickImage image = new MagickImage(file.OpenReadStream());
                image.AutoOrient();
                await memoryStream.WriteAsync(image.ToByteArray(), 0, image.ToByteArray().Count());
                memoryStream.Position = 0;
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
                await cloudBlockBlob.UploadFromStreamAsync(memoryStream);
            }
            else
            {
                await cloudBlobContainer.DeleteIfExistsAsync();
            }
        }
        // POST: api/Image
        [HttpPost]
        public async Task Post(IFormFile file, bool isReference, string jobId)
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=amitash;AccountKey=J0FlDqtF3fGnzU6p7Bb9WgE/yft4ycRbCCcQ+qsrO9O7Eroet676YZBRYrs0Tuxz2e2RZP6MrGi1XS0P6t45Qw==;EndpointSuffix=core.windows.net";

            if (CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    uploadBlob( file,  cloudBlobClient,
                        isReference ? "referenceimage" : jobId);
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
            }
        }
    }
}