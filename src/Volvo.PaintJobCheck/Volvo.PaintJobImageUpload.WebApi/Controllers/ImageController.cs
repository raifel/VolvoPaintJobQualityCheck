using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Volvo.PaintJobCheck.WebApi.Controllers
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

        // POST: api/Image
        [HttpPost]
        public async Task Post(IFormFile file)
        {

            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;

            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=paintcheckimages;AccountKey=qhuiRPu8Dr0ZKQzz6f68I+H2kIX+TRlNKfUWQLZDJx64KJR+Nck143VjsTLESA9UMzZc1X+hFY033vTynXAMWg==;EndpointSuffix=core.windows.net";

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
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    var directory = cloudBlobContainer.GetDirectoryReference(file.Headers["batchnumber"]);
                    CloudBlockBlob blockBlob = directory.GetBlockBlobReference(file.Headers["serialNo"]+".jpg");
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


        private static async Task ProcessAsync()
        {
            
        }
    }
}