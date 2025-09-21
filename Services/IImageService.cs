using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GoatSilencerArchitecture.Services
{
    public interface IImageService
    {
        Task<string> SaveAndCompressImage(IFormFile imageFile);
        void DeleteImageFiles(string filePath);
    }
}
