using System.IO;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
{
    public interface IBlobStorageService
    {
        // Sube un stream de imagen a blob storage y devuelve la ruta relativa (ej: "clientes/123/documentacion-a1b2c3.jpg")
        Task<string> UploadImageAsync(Stream fileStream, string folder, string fileName);
    }
}
