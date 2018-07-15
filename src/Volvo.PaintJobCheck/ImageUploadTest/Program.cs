using Volvo.VolvoPaintJobImageUpload.PictureUpload;
using System;
using System.IO;

namespace ImageUploadTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var filestream = File.OpenRead(@"C:\Users\shyam_hotmail\Desktop\test.jpg");
            var filestream2 = File.OpenRead(@"C:\Users\shyam_hotmail\Desktop\test.jpg");
            ApiService ap = new ApiService();
           var m= ap.UploadImageAsync(filestream, DateTime.Now.Ticks.ToString() + ".jpg").Result;
            ApiService ap2 = new ApiService();
           var x=  ap2.UploadImageAsync(filestream2, DateTime.Now.Ticks.ToString() + ".jpg").Result; 
        }
    }
}
