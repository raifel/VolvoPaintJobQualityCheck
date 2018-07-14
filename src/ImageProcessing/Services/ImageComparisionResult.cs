using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Auth;

namespace ImageProcessing.Services
{
    public class ImageComparisionResult
    {
        public string UploadImageAsync(HttpPostedFileBase imageToUpload)
        {
            string imageFullPath = null;
            if (imageToUpload == null || imageToUpload.ContentLength == 0)
            {
                return null;
            }

            try
            {
                CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("sampleimage");

                if (cloudBlobContainer.CreateIfNotExists())
                {
                    cloudBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                    );
                }

                string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
                cloudBlockBlob.UploadFromStream(imageToUpload.InputStream);

                imageFullPath = cloudBlockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {

            }

            return imageFullPath;
        }

        public void UploadImageAfterProcess(Bitmap image)
        {
            string connectionString =
                CloudConfigurationManager.GetSetting("imageprocessiong3_AzureStorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("imc1");
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var blockBlob = container.GetBlockBlobReference("Sampleblob.jpg");
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Jpeg);
                memoryStream.Seek(0, SeekOrigin.Begin); // otherwise you'll get zero byte files
                blockBlob.UploadFromStream(memoryStream);
            }

        }
    }
}

