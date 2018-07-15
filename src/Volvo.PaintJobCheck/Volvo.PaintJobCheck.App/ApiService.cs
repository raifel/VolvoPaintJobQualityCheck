using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace IC6.Xamarin.PictureUpload
{
    internal class ApiService 
    {
        //private string url = "http://10.233.32.147/WebApplication1/api/image";
        private string url = "http://10.233.32.147:3573/api/image";

        public HttpResponseMessage UploadImageAsync(Stream image, string fileName,bool reference,string batchNumber,int serialNo)
        {
            HttpContent fileStreamContent = new StreamContent(image);
            fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "file", FileName = fileName };
            fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            fileStreamContent.Headers.Add("isreference", reference.ToString());
            fileStreamContent.Headers.Add("batchnumber", batchNumber.ToString());
            fileStreamContent.Headers.Add("serialNo", serialNo.ToString());

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(fileStreamContent);
                var response =  client.PostAsync(url, formData).Result;
                return response;
            }
        }
    }
}