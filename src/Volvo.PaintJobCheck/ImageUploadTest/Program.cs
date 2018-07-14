using Volvo.VolvoPaintJobImageUpload.PictureUpload;
using System;
using System.IO;

namespace ImageUploadTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var filestream = File.OpenRead(@"C:\Users\a036540\Desktop\Shyamasundara Kudkuli.jpg");
            ApiService ap = new ApiService();
            ap.UploadImageAsync(filestream, "shyam.jpg").Wait();

        }
    }
}
