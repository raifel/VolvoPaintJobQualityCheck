using System.IO;
using System.Threading.Tasks;

namespace Volvo.VolvoPaintJobImageUpload.PictureUpload
{
    public interface IApiService
    {
        Task<bool> UploadImageAsync(Stream image, string fileName, bool reference, string batchNumber, int serialNo);
    }
}