using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
{
    public class LocalBlobStorageService : IBlobStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalBlobStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string folder, string fileName)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsRoot = Path.Combine(webRoot, "uploads");

            var normalizedFolder = string.IsNullOrWhiteSpace(folder)
                ? string.Empty
                : folder.Replace('/', Path.DirectorySeparatorChar).Trim(Path.DirectorySeparatorChar);

            var targetFolder = string.IsNullOrEmpty(normalizedFolder) ? uploadsRoot : Path.Combine(uploadsRoot, normalizedFolder);

            Directory.CreateDirectory(targetFolder);

            var filePath = Path.Combine(targetFolder, fileName);

            using (var dest = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileStream.CopyToAsync(dest);
            }

            var relative = Path.Combine("uploads", folder ?? string.Empty, fileName).Replace('\\', '/');
            return relative;
        }
    }
}
