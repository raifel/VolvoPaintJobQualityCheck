﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Volvo.VolvoPaintJobImageUpload.PictureUpload
{
    internal class ApiService : IApiService
    {
        private string url = "http://localhost:3574/api/image";

        public async Task<bool> UploadImageAsync(Stream image,  string fileName, bool reference=true, string batchNumber="sdf", int serialNo=1)
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
                var response = await client.PostAsync(url, formData);
                return response.IsSuccessStatusCode;
            }
        }
    }
}