using System.Drawing;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using motion;
using Volvo.PaintJobCheck.ImageProcessing;

namespace Volov.PaintJobCheck.ImageProcessingFuntion
{
    public static class ProcessPaintJobImages
    {

        public static string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=volvopaintjobstore;AccountKey=vJIvdg2K7AiqGYrlSloLbSCgPeQ5Aa+Csg8easpixGI7Kl1TqScQgJ89rY9cWcFP8jtAu8p1cTIRmmzB9N6gzw==;EndpointSuffix=core.windows.net";

        [FunctionName("ProcessPaintJobImages")]
        public static void Run([BlobTrigger("paintjobimages/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            ProcessAsync(name, log);
        }

        private static  void ProcessAsync(string path, TraceWriter log)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string referenedirectoryName = "reference";

            

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    cloudBlobContainer = cloudBlobClient.GetContainerReference("paintjobimages");
                    var blobref = cloudBlobContainer.GetBlobReference(path);
                    var referenedirectory = cloudBlobContainer.GetDirectoryReference(referenedirectoryName);
                    var currentJobDirectory = blobref.Parent;
                    if (currentJobDirectory is CloudBlobDirectory)
                    {
                        log.Info($"Started processing from directory {Path.GetDirectoryName(currentJobDirectory.Prefix) }");
                        if (Path.GetDirectoryName(currentJobDirectory.Prefix) == referenedirectoryName)
                            return;

                        var allJobImages = currentJobDirectory.ListBlobs();
                        int imageCount = 0;
                        foreach (var image in allJobImages)
                        {
                            imageCount++;
                        }
                        if (imageCount < 2 || imageCount > 2)
                        {
                            log.Info($"image count is in directory is { Path.GetDirectoryName(currentJobDirectory.Prefix) }: {imageCount}. So no processing");
                            return;
                        }

                        foreach (var image in allJobImages)
                        {
                            Bitmap referenceBitMap = GetBitMapFromImage(referenedirectory, Path.GetFileName(((CloudBlob)image).Name));
                            Bitmap jobBitMap = GetBitMapFromImage(currentJobDirectory, Path.GetFileName(((CloudBlob)image).Name));
                            Bitmap analyzeResult = CompareImages(referenceBitMap, jobBitMap);
                            if(analyzeResult!=null)
                            {

                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    analyzeResult.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                                    var resultImage = currentJobDirectory.GetBlockBlobReference($"Result{ Path.GetFileName(((CloudBlob)image).Name) } ");
                                    resultImage.UploadFromStream(memoryStream);
                                }
                            }
                            else
                            {
                                log.Info($"No difference found so not processed");
                            }
                        }
                    }
                }
                catch (StorageException ex)
                {
                    log.Info($"Error returned from the service: {ex.Message}");
                }
                finally
                {
                }
            }
            else
            {
                log.Info($"A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }

        private static Bitmap CompareImages(Bitmap referenceBitMap, Bitmap jobBitMap)
        {
            MotionDetector3 motionDetector = new MotionDetector3();
            motionDetector.MotionLevelCalculation = true;

            motionDetector.ProcessFrame(ref referenceBitMap);
            motionDetector.ProcessFrame(ref jobBitMap);

            if (motionDetector.MotionLevel > 0)
            {
                MotionDetector motionDetector1 = new MotionDetector();
                motionDetector1.ProcessFrame( referenceBitMap);
                Bitmap markedImage= motionDetector1.ProcessFrame( jobBitMap);
                return markedImage;
            }
            return null;
        }

        private static Bitmap GetBitMapFromImage(CloudBlobDirectory referenedirectory, string imageName)
        {

            CloudBlob referenceimage = referenedirectory.GetBlobReference(Path.GetFileName(imageName));
            Bitmap image;
            using (var memoryStream = new MemoryStream())
            {
                referenceimage.DownloadToStream(memoryStream);
                image = new Bitmap(memoryStream);
            }
            return image;
        }
    }

}

