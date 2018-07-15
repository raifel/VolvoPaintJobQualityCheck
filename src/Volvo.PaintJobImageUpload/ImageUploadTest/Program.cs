using Volvo.VolvoPaintJobImageUpload.PictureUpload;
using System;
using System.IO;

namespace ImageUploadTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var filestream = File.OpenRead(@"C:\Users\A056425\Desktop\front.jpeg");
            ApiService ap = new ApiService();
            ap.UploadImageAsync(filestream, "front.jpeg").Wait();

        }
    }
}
