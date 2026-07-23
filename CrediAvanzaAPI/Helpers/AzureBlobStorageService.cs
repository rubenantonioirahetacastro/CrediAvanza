using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorageConnectionString"]
                ?? throw new InvalidOperationException("No se encontró 'AzureBlobStorageConnectionString' en la configuración.");

            var containerName = configuration["AzureBlobStorageContainerName"] ?? "documentos";

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string folder, string fileName)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            var normalizedFolder = string.IsNullOrWhiteSpace(folder)
                ? string.Empty
                : folder.Trim('/');

            var blobName = string.IsNullOrEmpty(normalizedFolder)
                ? fileName
                : $"{normalizedFolder}/{fileName}";

            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobName;
        }
    }
}